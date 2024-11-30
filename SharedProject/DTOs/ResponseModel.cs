using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.DTOs
{
    public class ResponseModel<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseModel(bool status, string msg, T data)
        {
            this.Status = status;
            this.Message = msg;
            this.Data = data;
        }

        public ResponseModel(){}
    }

    public class ResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ResponseModel(bool status, string msg, object data)
        {
            this.Status = status;
            this.Message = msg;
            this.Data = data;
        }

        public ResponseModel() { }
    }
}
