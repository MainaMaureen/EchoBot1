// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using EchoBot1.Bots;
using EchoBot1.Services;
using EchoBot1.Dialogs;

namespace EchoBot1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

             // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //Configure State
            ConfigureState(services);

            //Configure Dialogs
            ConfigureDialogs(services);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();  //MainDialogs is where we start
        }

        public void ConfigureState(IServiceCollection services)
        {
            //Create the storage we will be using for the User and Conversation state. (Memory is great for testing purposes.)
            // services.AddSingleton<IStorage, MemoryStorage>();

            //Saving data to Azure Blob Storage
            var storageAccount = "DefaultEndpointsProtocol=https;AccountName=feedbackbot;AccountKey=zdYldjRDkzAG33x3/JiObsyq8u6YKPr2SIDmucAuIo/VGwK/NYKw3acYL5eD4xLdfAwyRVdNDNlFTMv+oa8rAA==;EndpointSuffix=core.windows.net";
             var storageContainer = "mystatedata";
             services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));

            //Create the User state.
            services.AddSingleton<UserState>();

            //Create the Conversation state.
            services.AddSingleton<ConversationState>();

            //Create an instance of the state service.
            services.AddSingleton<BotStateService>();
        }

        public void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>(); 
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
