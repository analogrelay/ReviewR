using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace ReviewR.Web.Models.Response
{
    public class FormResponseModel<T>
    {
        public bool Success { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
        public T Result { get; set; }

        public FormResponseModel(ModelStateDictionary state) : this(state, default(T)) { }
        public FormResponseModel(T result) : this(null, result) { }
        public FormResponseModel(ModelStateDictionary state, T result)
        {
            Success = state != null && !state.IsValid;
            Result = result;
            if (state != null)
            {
                Errors = state.ToDictionary(p => p.Key, p => p.Value.Errors.Select(e => e.ErrorMessage).ToArray());
            }
        }
    }
}