using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using WikiObjects.Data.Model;

namespace WikiObjects.Data.ModelInterface
{
    public class BaseInterface<T, S> where T: MongoModel where S : IApplyModel<T, S>, new()
    {
        public static S GetById(string id)
        {
            T one = DB.Find<T>()
                .One(id);

            var s = new S();
            s.ApplyModel(one);
            return s;
        }
    }
}
