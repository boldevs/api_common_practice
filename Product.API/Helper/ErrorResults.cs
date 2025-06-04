namespace Product.API.Helpers
{
    public static class ErrorResults
    {
        public static IResult NotFound(string detail, HttpContext http) =>
            Results.Problem(
                statusCode: 404,
                title: "Not Found",
                detail: detail,
                type: "https://httpstatuses.com/404",
                instance: http.Request.Path);

        public static IResult BadRequest(string detail, HttpContext http) =>
            Results.Problem(
                statusCode: 400,
                title: "Bad Request",
                detail: detail,
                type: "https://httpstatuses.com/400",
                instance: http.Request.Path);

        public static IResult Conflict(string detail, HttpContext http) =>
            Results.Problem(
                statusCode: 409,
                title: "Conflict",
                detail: detail,
                type: "https://httpstatuses.com/409",
                instance: http.Request.Path);

        public static IResult InternalError(string detail, HttpContext http) =>
            Results.Problem(
                statusCode: 500,
                title: "Internal Server Error",
                detail: detail,
                type: "https://httpstatuses.com/500",
                instance: http.Request.Path);
    }
}
