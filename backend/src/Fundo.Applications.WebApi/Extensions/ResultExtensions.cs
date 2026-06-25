using Fundo.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return controller.NoContent();
            }

            return ToErrorActionResult(controller, result.Error);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(result.Value);
            }

            return ToErrorActionResult(controller, result.Error);
        }

        public static IActionResult ToCreatedActionResult<T>(
            this Result<T> result,
            ControllerBase controller,
            string routeName,
            object routeValues)
        {
            if (result.IsSuccess)
            {
                return controller.CreatedAtRoute(routeName, routeValues, result.Value);
            }

            return ToErrorActionResult(controller, result.Error);
        }

        private static IActionResult ToErrorActionResult(ControllerBase controller, Error error)
        {
            return error switch
            {
                ValidationError validationError => controller.ValidationProblem(
                    validationError,
                    StatusCodes.Status400BadRequest),

                _ when error.Type == ErrorType.NotFound => controller.Problem(
                    detail: error.Description,
                    statusCode: StatusCodes.Status404NotFound,
                    title: error.Code),

                _ when error.Type == ErrorType.Validation => controller.Problem(
                    detail: error.Description,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: error.Code),

                _ => controller.Problem(
                    detail: error.Description,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: error.Code)
            };
        }

        private static IActionResult ValidationProblem(
            this ControllerBase controller,
            ValidationError validationError,
            int statusCode)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = statusCode,
                Title = validationError.Code,
                Detail = validationError.Description
            };

            foreach (Error item in validationError.Errors)
            {
                string key = string.IsNullOrWhiteSpace(item.Code) ? "Error" : item.Code;
                problemDetails.Errors.Add(key, new[] { item.Description });
            }

            return controller.ValidationProblem(problemDetails);
        }
    }
}
