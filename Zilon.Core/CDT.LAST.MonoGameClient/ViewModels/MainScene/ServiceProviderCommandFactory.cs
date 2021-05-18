using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Commands;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class ServiceProviderCommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderCommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TCommand GetCommand<TCommand>() where TCommand : ICommand
        {
            return _serviceProvider.GetRequiredService<TCommand>();
        }
    }
}
