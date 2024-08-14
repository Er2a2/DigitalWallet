using DigitalWallet.Commmon;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Extensions;
using FluentValidation;
using DataAnnotationsValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using DIgitalWallet.Commmon.Utility;


namespace DigitalWallet.Middlewares
{
    public static class CustomExceptionMiddlewareExtension
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ISerilogService>();
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                context.Response.StatusCode = 400;
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    ?.Error;

                ErrorResponse error;

                if (exception is DIgitalWallet.Commmon.Utility.AppException appException)
                {
                    error = new ErrorResponse
                    {
                        Code = appException.Code,
                        Type = DIgitalWallet.Commmon.Utility.EnumExtensions.GetDisplayName((ExceptionCode)appException.Code),
                        Message = string.Format(DIgitalWallet.Commmon.Utility.EnumExtensions.GetDescription((ExceptionCode)appException.Code), appException.MessageParams),
                        AdditionalData = appException.AdditionalData
                    };
                }
                else if (exception is FluentValidation.ValidationException validationException)
                {
                    var exceptionCode = ExceptionCode.ValidationException;
                    error = new ErrorResponse()
                    {
                        Code = (int)exceptionCode,
                        Type = DIgitalWallet.Commmon.Utility.EnumExtensions.GetDisplayName(exceptionCode),
                        Message = validationException.Message,
                        AdditionalData = validationException.Errors.Select(x => new { Value = x.AttemptedValue, Message = x.ErrorMessage }).ToList()
                    };
                }
                else
                {
                    var exceptionCode = ExceptionCode.SystemException;
                    error = new ErrorResponse()
                    {
                        Code = (int)exceptionCode,
                        Message = exceptionCode.GetDescription(),
                        Type = DIgitalWallet.Commmon.Utility.EnumExtensions.GetDisplayName(exceptionCode),
                    };
                }
                await context.Response.WriteAsJsonAsync(error);
                logger.CustomLog(logLevel: LogLevel.Error, source: "AppException", serviceName: error.Type, description: $"Message: {error.Message} ** AdditionalData: {error.AdditionalData}");
            }));
        }

        public static string JoinParamsWithComma(object[] parameters)
        {
            if (parameters.Length == 0)
                return null;

            var stringParams = parameters.Select(p => p.ToString()).ToArray();
            return string.Join(", ", stringParams);
        }

        private class ErrorResponse
        {
            public string Type { get; set; }
            public int Code { get; set; }
            public string Message { get; set; }
            public object? AdditionalData { get; set; }
        }
    }
}
