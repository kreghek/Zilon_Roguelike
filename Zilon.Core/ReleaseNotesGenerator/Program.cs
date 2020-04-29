using System;
using System.Net.Http;
using Octokit;
using Zilon.CommonUtilities;

namespace ReleaseNotesGenerator
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var client = new GitHubClient(new ProductHeaderValue("Zilon_Roguelike"));

            client.Issue.Get("Zilon_Roguelike", 781);

            var responce = await client.GetAsync("/repos/kreghek/Zilon_Roguelike/issues/781");
            var responsetext = await responce.Content.ReadAsStringAsync();
        }
    }
}
