using AB.Extensions;
using ApiKickstart.WebApi.Views;
using System;

namespace ApiKickstart.WebApi.Response
{
    public class ApiResponse<T> where T : class  //more on generic constraints @ https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters
    {
        public Meta Meta { get; set; }
        public T Result { get; set; }
        
    }

    public class Meta
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public PagingResponse Paging { get; set; } = null;
    }

    public class ApiResponse
    {
        public static ApiResponse<T> ApiResponseFactory<T>(T resultObject, PagingResponse pagingResponse = null) where T : class
        {
            var response = new ApiResponse<T>();
            response.Result = resultObject;
            response.Meta = new Meta();
            response.Meta.Paging = pagingResponse;
            return response;
        }        
    }
}
