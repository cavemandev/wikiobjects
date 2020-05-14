using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

using MemberListDict = System.Collections.Generic.Dictionary<string, WikiObjects.Data.ModelInterface.MemberType>;

namespace WikiObjects.Data.ModelInterface
{
    public enum MemberList
    {
        readers = 1,
        admins = 2,
    }

    public enum MemberType
    {
        user = 1,
        team = 2,
        owner = 3,
    }

    public class ACLInterface<T> where T : ACLObject
    {
        private static string ACLMemberString(string id, MemberList type)
        {
            string memberListType = type.ToString("g");
            return string.Format("acl.{0}.{1}", memberListType, id);
        }

        private static MemberListDict CombineMemberLists(MemberListDict list1, MemberListDict list2)
        {
            var outList = new MemberListDict();
            list1.ToList().ForEach(k => outList.Add(k.Key, k.Value));

            list2.ToList().ForEach(k =>
            {
                if (!outList.ContainsKey(k.Key))
                {
                    outList.Add(k.Key, k.Value);
                }
            });

            return outList;
        }

        public static void ChangeOwner(string objectId, User newOwner)
        {
            DB.Update<Team>()
                .Match(a => a.ID == objectId)
                .Modify(a => a.Set("acl.ownerId", newOwner.ID))
                .Execute();
        }

        public static void AddMember(T aclObject, User member, MemberList type)
        {
            AddMember(aclObject.ID, member, type);
        }

        public static void AddMember(string aclObjectId, ACLMember member, MemberList type)
        {
            MemberList otherType = type == MemberList.admins ? MemberList.readers : MemberList.admins;

            MemberType memberType = (member as User) != null ? MemberType.user : MemberType.team;

            // TODO: Check to see if member is an owner, if so, don't do anything
            DB.Update<Team>()
                .Match(a => a.ID == aclObjectId)
                .Match(a => a.acl.ownerId != member.ID)
                .Modify(a => a.Set(ACLMemberString(member.ID, type), memberType))
                .Modify(a => a.Unset(ACLMemberString(member.ID, otherType)))
                .Execute();
        }

        public static void RemoveMember(T aclObject, ACLMember member)
        {
            RemoveMember(aclObject.ID, member);
        }

        public static void RemoveMember(string aclObjectId, ACLMember member)
        {
            DB.Update<T>()
                .Match(a => a.ID == aclObjectId)
                .Modify(a => a.Unset(string.Format("acl.{0}.{1}", MemberList.readers.ToString("g"), member.ID)))
                .Modify(a => a.Unset(string.Format("acl.{0}.{1}", MemberList.admins.ToString("g"), member.ID)))
                .Execute();
        }

        public static bool IsOwner(T aclObject, User member)
        {
            return IsOwner(aclObject.ID, member);
        }

        public static bool IsOwner(string aclObjectId, User member)
        {
            var teams = DB.Find<T>()
                .Match(aclObjectId)
                .Match(t => t.acl.ownerId == member.ID)
                .Execute();

            return teams != null;
        }

        public static bool IsAdmin(T aclObject, ACLMember member)
        {
            return IsAdmin(aclObject.ID, member);
        }

        public static bool IsAdmin(string aclObjectId, ACLMember member)
        {
            var team = DB.Find<T>()
                .One(aclObjectId);

            // Check to see if the member is the owner, or just in the admins list
            if (team.acl.ownerId == member.ID || team.acl.admins.ContainsKey(member.ID))
            {
                return true;
            }

            var teams = team.acl.admins.Where(a => a.Value == MemberType.team).Select(a => a.Key).ToList();

            var admins = new MemberListDict();
            teams.ForEach(teamId =>
            {
                admins = CombineMemberLists(admins, GetTeamMembership(teamId));
            });

            return admins.ContainsKey(member.ID);
        }

        public static bool IsReader(T aclObject, ACLMember member)
        {
            return IsReader(aclObject.ID, member);
        }

        public static bool IsReader(string aclObjectId, ACLMember member)
        {
            var team = DB.Find<T>()
                .One(aclObjectId);

            // Check to see if the member is the owner, or just in the admins or readers lists
            if (team.acl.ownerId == member.ID || team.acl.admins.ContainsKey(member.ID) || team.acl.readers.ContainsKey(member.ID))
            {
                return true;
            }

            var teams = team.acl.admins.Where(a => a.Value == MemberType.team).Select(a => a.Key).ToList();
            teams = teams.Concat(team.acl.readers.Where(a => a.Value == MemberType.team).Select(a => a.Key).ToList()).ToList();

            var readers = new MemberListDict();
            teams.ForEach(teamId =>
            {
                readers = CombineMemberLists(readers, GetTeamMembership(teamId));
            });

            return readers.ContainsKey(member.ID);
        }

        public static MemberListDict GetTeamMembership(string teamId)
        {
            var memberShip = new MemberListDict();
            
            var team = DB.Find<Team>()
                .One(teamId);
            
            if (team != null)
            {
                memberShip.Add(team.acl.ownerId, MemberType.owner);

                memberShip = CombineMemberLists(memberShip, team.acl.readers);
                memberShip = CombineMemberLists(memberShip, team.acl.admins);
                
                HashSet<string> moreTeams = new HashSet<string>();
                team.acl.readers.Where(t => t.Value == MemberType.team).ToList().ForEach(t => moreTeams.Add(t.Key));
                team.acl.admins.Where(t => t.Value == MemberType.team).ToList().ForEach(t => moreTeams.Add(t.Key));

                moreTeams.ToList().ForEach(teamId =>
                {
                    var moreMembers = GetTeamMembership(teamId);
                    memberShip = CombineMemberLists(memberShip, moreMembers);
                });
            }

            return memberShip;
        }
    }
}
