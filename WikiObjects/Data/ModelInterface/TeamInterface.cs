using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class Team : ACLContainer, IAclMember, IApplyModel<TeamModel, Team>
    {
        public static string AdminTeamName = "admin";
        public static Team FromModel(TeamModel um)
        {
            return new Team() { Id = um.ID, Name = um.name, Acl = um.acl };
        }

        public void ApplyModel(TeamModel um)
        {
            Id = um.ID;
            Name = um.name;
            Acl = um.acl;
        }

        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class TeamInterface : AclInterface<TeamModel, Team>, IAclMembershipInterface
    {
        private enum UpdateFields
        {
            name,
        }

        public Team Create(string name, User owner)
        {
            TeamModel team = new TeamModel(owner.Id) { name = name };
            team.Save();
            return Team.FromModel(team);
        }

        public Team GetByName(string name)
        {
            var teams = DB.Find<TeamModel>()
                .Match(t => t.name.Equals(name))
                .Limit(1)
                .Execute();

            return teams.Count > 0 ? Team.FromModel(teams.FirstOrDefault()) : null;
        }

        public long Delete(string teamId)
        {
            return DB.Delete<TeamModel>(teamId).DeletedCount;
        }

        public Team Update(string teamId, Dictionary<string, string> updates)
        {
            TeamModel teamModel = DB.Find<TeamModel>().One(teamId);

            if (teamModel == null)
            {
                return null;
            }

            if (updates.ContainsKey(UpdateFields.name.ToString()))
            {
                teamModel.name = updates[UpdateFields.name.ToString()];
            }

            teamModel.Save();

            return Team.FromModel(teamModel);
        }

        public bool Permissions(string teamId)
        {
            // TODO: Search for teams with teamId
            // recursivily search upper teams until no more teams
            var teams = GetAssociatedTeams(teamId);
            // teams.ForEach(team => );
            return true;
        }

        public List<TeamModel> GetAssociatedTeams(string memberId)
        {
            string adminMember = string.Format("Acl.Admins.{0}", memberId);
            string readerMember = string.Format("Acl.Readers.{0}", memberId);

            var teams = DB.Find<TeamModel>()
                .Match(t => t.Eq(x => x.acl.ownerId, memberId) |
                    t.Exists(adminMember) |
                    t.Exists(readerMember))
                .Project(t => t.Include(adminMember).Include(readerMember))
                .Execute();

            return teams;
        }
    }
}
