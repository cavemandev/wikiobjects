using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class TeamInterface : ACLInterface<Team>
    {
        public static Team Create(string name, User owner)
        {
            Team team = new Team(owner) { name = name };
            team.Save();
            return team;
        }

        public static Team GetByName(string name)
        {
            var teams = DB.Find<Team>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return teams.Count > 0 ? teams.FirstOrDefault() : null;
        }

        public static long Delete(string teamId)
        {
            return DB.Delete<Team>(teamId).DeletedCount;
        }

        public static bool Permissions(string teamId)
        {
            // TODO: Search for teams with teamId
            // recursivily search upper teams until no more teams
            var teams = GetAssociatedTeams(teamId);
            // teams.ForEach(team => );
            return true;
        }

        public static List<Team> GetAssociatedTeams(string memberId)
        {
            string adminMember = string.Format("Acl.Admins.{0}", memberId);
            string readerMember = string.Format("Acl.Readers.{0}", memberId);

            var teams = DB.Find<Team>()
                .Match(t => t.Eq(x => x.acl.ownerId, memberId) |
                    t.Exists(adminMember) |
                    t.Exists(readerMember))
                .Project(t => t.Include(adminMember).Include(readerMember))
                .Execute();

            return teams;
        } 
    }
}
