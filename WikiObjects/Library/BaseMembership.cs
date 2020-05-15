using WikiObjects.Data.Model;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library.Exceptions;

namespace WikiObjects.Library
{
    public class BaseMembership
    {
        protected IAclMembershipInterface membershipInterface;
        protected UserInterface userInterface = new UserInterface();
        protected TeamInterface teamInterface = new TeamInterface();

        protected BaseMembership(IAclMembershipInterface modelInterface)
        {
            membershipInterface = modelInterface;
        }

        public void ChangeOwner(string id, string newOwnerId, User subject)
        {
            if (!membershipInterface.IsAdmin(id, subject))
            {
                throw new NotAuthorized();
            }

            var newOwner = userInterface.GetById(newOwnerId);

            if (newOwner == null)
            {
                throw new InvalidArugment();
            }

            membershipInterface.ChangeOwner(id, newOwner);
        }

        public bool AddAdmin(string id, string newAdminId, User subject)
        {
            if (!membershipInterface.IsAdmin(id, subject))
            {
                throw new NotAuthorized();
            }

            IAclMember newAdmin = userInterface.GetById(newAdminId);
            if (newAdmin == null)
            {
                newAdmin = teamInterface.GetById(newAdminId);
            }

            if (newAdmin == null)
            {
                return false;
            }

            return membershipInterface.AddMember(id, newAdmin, MemberList.admins);
        }

        public bool AddReader(string id, string newAdminId, User subject)
        {
            if (!membershipInterface.IsAdmin(id, subject))
            {
                throw new NotAuthorized();
            }

            IAclMember newAdmin = userInterface.GetById(newAdminId);
            if (newAdmin == null)
            {
                newAdmin = teamInterface.GetById(newAdminId);
            }

            if (newAdmin == null)
            {
                return false;
            }

            return membershipInterface.AddMember(id, newAdmin, MemberList.readers);
        }

        public void RemoveMember(string id, string unMemberId, User subject)
        {
            if (!membershipInterface.IsAdmin(id, subject))
            {
                throw new NotAuthorized();
            }

            IAclMember unMember = userInterface.GetById(unMemberId);
            if (unMember == null)
            {
                unMember = teamInterface.GetById(unMemberId);
            }

            if (unMember == null)
            {
                return;
            }

            membershipInterface.RemoveMember(id, unMember);
        }
    }
}
