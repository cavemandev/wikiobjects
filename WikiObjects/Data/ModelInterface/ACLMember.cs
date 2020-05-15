namespace WikiObjects.Data.Model
{
    public interface IAclMember
    {
        public string Id { get; set; }
    }

    public interface IApplyModel<T, S> where T: MongoModel
    {
        public void ApplyModel(T t);
    }

    public class ACLContainer
    {
        public Acl Acl { get; set; }
    }
}
