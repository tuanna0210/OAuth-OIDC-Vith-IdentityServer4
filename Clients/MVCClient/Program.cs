namespace MVCClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            //Config OIDC
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = builder.Configuration["Authentication:AuthorizationCodeSetting:AuthorityUrl"];
                options.ClientId = builder.Configuration["Authentication:AuthorizationCodeSetting:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:AuthorizationCodeSetting:ClientSecret"];
               
                var listScope = builder.Configuration.GetSection("Authentication:AuthorizationCodeSetting:Scopes").Get<List<string>>();
                listScope.ForEach(s =>
                {
                    options.Scope.Add(s);
                });

                options.ResponseType = "code";
                options.UsePkce = true;
                options.ResponseMode = "query";
                options.SaveTokens = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            //app.MapRazorPages();

            app.Run();
        }
    }
}