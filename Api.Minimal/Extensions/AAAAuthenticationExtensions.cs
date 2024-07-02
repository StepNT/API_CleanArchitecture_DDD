using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;

namespace Api.Minimal.Extensions;

public static partial class AAAAuthenticationExtensions
{
    public static WebApplicationBuilder AddAAAAuthentication(this WebApplicationBuilder builder)
    {
        IConfigurationRoot _config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string PolicyAUTH = "api_access";

        builder
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = _config["OidcConfiguration:Url"];
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidTypes = ["at+jwt"];
            });

        builder
            .Services.AddAuthorizationBuilder()
            .AddPolicy(
                PolicyAUTH,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", _config["OidcConfiguration:Scope"]);
                }
            );

        return builder;
    }

    public static IApplicationBuilder UseAAAAuthentication(
        this IApplicationBuilder applicationBuilder
    )
    {
        applicationBuilder
            .UseAuthentication()
            .UseAuthorization()
            .UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    RequireHeaderSymmetry = false,
                    ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                }
            );

        return applicationBuilder;
    }
}
