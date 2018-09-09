using System;
using ApiKickstart.CQRS;
using Microsoft.AspNetCore.Mvc;
using ApiKickstart.Queries;
using AutoMapper;
using ApiKickstart.WebApi.Views;

namespace ApiKickstart.WebApi.Response
{
    public static class ControllerExtensions
    {
        // this is a good starting point but will need further refactoring for more betterness
        public static IActionResult ReturnObjectResult<TView,TQueryResult>(this ControllerBase controller, TQueryResult queryResult, IMapper mapper)
            where TView : class
            where TQueryResult : class, IQueryResult
        {
            var viewModel = mapper.Map<TView>(queryResult); // todo: can we remove mapper as a parameter? 
            var responseObject = ApiResponse.ApiResponseFactory<TView>(viewModel);
            switch (queryResult.ResultStatus)
            {
                case QueryResultStatus.NotYetProcessed:
                    throw new Exception("A query result has been returned to the controller in its default state.  Implement a try-catch-finally and set the appropriate status.");
                case QueryResultStatus.SuccessfullyProcessed:
                    return new OkObjectResult(responseObject);
                case QueryResultStatus.NoResultData:
                    return new NotFoundObjectResult(responseObject);
                case QueryResultStatus.CriticalError:
                    return new ObjectResult(responseObject) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
                default: throw new ArgumentException("The result enum has no appropriate HTTP status code mapped.");
            }
        }

        public static IActionResult ReturnObjectResult<TView, TQueryResult>(this ControllerBase controller, IPagedQueryResult queryResult, IMapper mapper)
            where TView : class, IPagedResultView
            where TQueryResult : class, IPagedQueryResult
        {
            var viewModel = mapper.Map<TView>(queryResult); // todo: can we remove mapper as a parameter? 
            var responseObject = ApiResponse.ApiResponseFactory<TView>(viewModel, queryResult.Paging);
            switch (queryResult.ResultStatus)
            {
                case QueryResultStatus.NotYetProcessed:
                    throw new Exception("A query result has been returned to the controller in its default state.  Implement a try-catch-finally and set the appropriate status.");
                case QueryResultStatus.SuccessfullyProcessed:
                    return new OkObjectResult(responseObject);
                case QueryResultStatus.NoResultData:
                    return new NotFoundObjectResult(responseObject);
                case QueryResultStatus.CriticalError:
                    return new ObjectResult(responseObject) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
                default: throw new ArgumentException("The result enum has no appropriate HTTP status code mapped.");
            }
        }

        // this is a good starting point but will need further refactoring for more betterness
        public static IActionResult ReturnCreatedCommandResult<TView, TCommand>(this ControllerBase controller, TCommand commandResult, IMapper mapper, string redirectActionName, IQuery redirectQuery)
            where TView : class
            where TCommand : class, ICommandResult
        {
            var viewModel = mapper.Map<TView>(commandResult); //todo clean up this random reference
            var responseObject = ApiResponse.ApiResponseFactory<TView>(viewModel);
            switch (commandResult.ResultStatus)
            {
                case CommandResultStatus.NotYetProcessed:
                    throw new Exception("A query result has been returned to the controller in its default state.  Implement a try-catch-finally and set the appropriate status.");
                case CommandResultStatus.ServiceUnavailable:
                    return new StatusCodeResult(503); // todo: generic hypermedia for service unavailable
                case CommandResultStatus.SuccessfullyProcessed:
                    return new CreatedAtActionResult(redirectActionName, controller.ControllerContext.ActionDescriptor.ControllerName, redirectQuery, responseObject);
                case CommandResultStatus.CriticalError:
                    return new ObjectResult(responseObject) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
                default: throw new ArgumentException("The result enum has no appropriate HTTP status code mapped.");
            }
        }

        //public static IActionResult ReturnStatusCodeForCommandResult<T>(this ControllerBase controller, ApiResponse<T> response, string actionName, IQuery redirectQuery) where T : class, ICommandResult
        //{
        //    switch (response.Result.ResultStatus)
        //    {
        //        case CommandResultStatus.NotYetProcessed:
        //            throw new Exception("A query result has been returned to the controller in its default state.  Implement a try-catch-finally and set the appropriate status.");
        //        case CommandResultStatus.SuccessfullyProcessed:
        //            return new CreatedAtActionResult (actionName, controller.ControllerContext.ActionDescriptor.ControllerName, redirectQuery, response);
        //        case CommandResultStatus.ServiceUnavailable:
        //            return new StatusCodeResult(503);//todo: service unavailable results
        //        case CommandResultStatus.CriticalError:
        //            return new StatusCodeResult(500);
        //        default: throw new ArgumentException("The result enum has no appropriate HTTP status code mapped.");
        //    }
        //}

        /// <summary>
        /// PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="response"></param>
        /// <param name="actionName"></param>
        /// <param name="redirectQuery"></param>
        /// <returns></returns>
        public static IActionResult ReturnStatusCodeForPutCommandResult<T>(this ControllerBase controller, ApiResponse<T> response) where T : class, ICommandResult
        {
            switch (response.Result.ResultStatus)
            {
                case CommandResultStatus.NotYetProcessed:
                    throw new Exception("A query result has been returned to the controller in its default state.  Implement a try-catch-finally and set the appropriate status.");
                case CommandResultStatus.SuccessfullyProcessed:
                    return new OkObjectResult(response);
                case CommandResultStatus.ServiceUnavailable:
                    return new StatusCodeResult(503);//todo: service unavailable results
                case CommandResultStatus.CriticalError:
                    return new StatusCodeResult(500);
                default: throw new ArgumentException("The result enum has no appropriate HTTP status code mapped.");
            }
        }

        public static TDestination Map<TSource, TDestination>(
            this TDestination destination, TSource source)
        {
            return Mapper.Map(source, destination);
        }
    }
}
