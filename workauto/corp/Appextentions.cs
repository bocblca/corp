
public static class AppExtensions
{
    public static IApplicationBuilder StartJob(this IApplicationBuilder app) {

        var scope=app.ApplicationServices.CreateScope();
        var startJob =scope.ServiceProvider.GetRequiredService<StartHJob>();
        startJob.Starthangfirejob();
        return app; 
    } 
}
