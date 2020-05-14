namespace WikiObjects.Data.Model
{
    public interface IACLMember
    {
        public string Id { get; set; }
    }

    public interface IApplyModel<T, S> where T: MongoModel
    {
        public void ApplyModel(T t);
    }

    public class ACLContainer
    {
        public ACL Acl { get; set; }
    }
}
