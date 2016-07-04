using Recardo.EnterpriseServices.globe.Contracts;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.TableStandl;
using System.TableStandl.Configuration;
using System.TableStandl.Description;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IWcfServiceProxy<TService>
    {
        /// <summary>
        ///     Configures the internal WCF channel factory from an action delegate.
        /// </summary>
        /// <param name="channelFactoryConfigurator">
        ///     Action delegate to use for configuring the channel
        ///     factory.
        /// </param>
        void ConfigureChannelFactory(Action<ChannelFactory<TService>> channelFactoryConfigurator);

        /// <summary>
        ///     Provides basic username/password credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void SetUsernameCredentials(string username, string password);

        /// <summary>
        ///     Provides Windows credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="credential"></param>
        void SetWindowsCredentials(NetworkCredential credential);

        /// <summary>
        ///     Provides Windows credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        void SetWindowsCredentials(string username, string password, string domain);

        /// <summary>
        ///     Performs the specified action with the service proxy.
        /// </summary>
        /// <param name="action">Action delegate to perform.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        void Use(Action<TService> action);

        /// <summary>
        ///     Performs the specified function with the service proxy.
        /// </summary>
        /// <param name="action">Function delegate to perform.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        TResult Use<TResult>(Func<TService, TResult> action);

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that requires a request parameter.
        /// </summary>
        /// <typeparam name="TRequest">Type of request used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="request">Request to pass to the beginning action delegate.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        void UseAsync<TRequest>(Func<TService, TRequest, AsyncCallback, object, IAsyncResult> beginAction,
            TRequest request, Func<TService, IAsyncResult> endAction, Action callback);

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that returns a response.
        /// </summary>
        /// <typeparam name="TResponse">Type of response used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        void UseAsync<TResponse>(Func<TService, AsyncCallback, object, IAsyncResult> beginAction,
            Func<TService, IAsyncResult, TResponse> endAction, Action<TResponse> callback);

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that requires a request parameter and returns a response.
        /// </summary>
        /// <typeparam name="TRequest">Type of request used by the operation.</typeparam>
        /// <typeparam name="TResponse">Type of response used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="request">Request to pass to the beginning action delegate.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        void UseAsync<TRequest, TResponse>(
            Func<TService, TRequest, AsyncCallback, object, IAsyncResult> beginAction, TRequest request,
            Func<TService, IAsyncResult, TResponse> endAction, Action<TResponse> callback);
    }

    /// <summary>
    ///     Serves as a communication proxy between a client application and a web service using
    ///     Windows Communication Foundation.
    /// </summary>
    /// <typeparam name="TService">Type of service for which to serve as a client.</typeparam>
    [DebuggerNonUserCode]
    internal class WcfServiceProxy<TService> : IWcfServiceProxy<TService>
    {
        private readonly ChannelFactory<TService> _channelFactory;

        /// <summary>
        ///     Creates a new instance, initializing the channel factory from an endpoint defined in the
        ///     application configuration.
        /// </summary>
        /// <param name="endpointConfigurationName">
        ///     Name of the service endpoint configuration as stored in
        ///     the system.TableStandl section of the application configuration file.
        /// </param>
        public WcfServiceProxy(string endpointConfigurationName)
        {
            this._channelFactory = new ChannelFactory<TService>(endpointConfigurationName);
        }

        /// <summary>
        ///     Creates a new instance, initializing the channel factory from an endpoint object.
        /// </summary>
        /// <param name="endpoint">Service endpoint for the client.</param>
        public WcfServiceProxy(ServiceEndpoint endpoint)
        {
            this._channelFactory = new ChannelFactory<TService>(endpoint);
        }

        /// <summary>
        ///     Initializes a new instance, inspecting the config file for automatically determining the endpoint configuration.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     No endpoint can
        ///     be found with the contract type <see cref="IglobeService" />, or
        ///     multiple endpoints were found with the contract type
        ///     <see cref="IglobeService" />.
        /// </exception>
        /// <remarks>
        ///     Use this constructor when there is exactly one target endpoint in
        ///     the application configuration file.
        /// </remarks>
        public WcfServiceProxy()
        {
            var contractType = typeof(TService);
            var section = ConfigurationManager.GetSection("system.TableStandl/client") as ClientSection;
            var eps = section.ElementInformation.Properties[string.Empty].Value as ChannelEndpointElementCollection;
            var found = false;
            foreach (ChannelEndpointElement ep in eps)
            {
                if (contractType.FullName == ep.Contract)
                {
                    if (!found)
                    {
                        this._channelFactory = new ChannelFactory<TService>(ep.Name);
                        found = true;
                    }
                    else
                    {
                        var message =
                            $"More than one endpoint was found which satisfies the contract '{contractType}'. Remove all but one of these endpoints or specify an endpoint name in the constructor of {this.GetType()}.";
                        throw new InvalidOperationException(message);
                    }
                }
            }

            if (!found)
            {
                var message = $"No endpoint could be found which satisfies the contract '{contractType}'.";
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        ///     Configures the internal WCF channel factory from an action delegate.
        /// </summary>
        /// <param name="channelFactoryConfigurator">
        ///     Action delegate to use for configuring the channel
        ///     factory.
        /// </param>
        public void ConfigureChannelFactory(Action<ChannelFactory<TService>> channelFactoryConfigurator)
        {
            channelFactoryConfigurator(this._channelFactory);
        }

        /// <summary>
        ///     Provides basic username/password credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SetUsernameCredentials(string username, string password)
        {
            this._channelFactory.Credentials.UserName.UserName = username;
            this._channelFactory.Credentials.UserName.Password = password;
        }

        /// <summary>
        ///     Provides Windows credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="credential"></param>
        public void SetWindowsCredentials(NetworkCredential credential)
        {
            this._channelFactory.Credentials.Windows.ClientCredential = credential;
        }

        /// <summary>
        ///     Provides Windows credentials to any requests made by the service proxy.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        public void SetWindowsCredentials(string username, string password, string domain)
        {
            this._channelFactory.Credentials.Windows.ClientCredential =
                new NetworkCredential(username, password, domain);
        }

        /// <summary>
        ///     Performs the specified action with the service proxy.
        /// </summary>
        /// <param name="action">Action delegate to perform.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        public void Use(Action<TService> action)
        {
            this.Use(s =>
            {
                action(s);
                return true;
            });
        }

        /// <summary>
        ///     Performs the specified function with the service proxy.
        /// </summary>
        /// <param name="action">Function delegate to perform.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        public TResult Use<TResult>(Func<TService, TResult> action)
        {
            var client = this._channelFactory.CreateChannel();
            var channel = (IClientChannel)client;

            try
            {
                channel.Open();
                return action(client);
            }
            finally
            {
                if (channel != null)
                {
                    try
                    {
                        channel.Close();
                    }
                    catch
                    {
                        channel.Abort();
                    }
                }
            }
        }

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that requires a request parameter.
        /// </summary>
        /// <typeparam name="TRequest">Type of request used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="request">Request to pass to the beginning action delegate.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        public void UseAsync<TRequest>(Func<TService, TRequest, AsyncCallback, object, IAsyncResult> beginAction,
            TRequest request, Func<TService, IAsyncResult> endAction, Action callback)
        {
            var client = this._channelFactory.CreateChannel();
            var channel = (IClientChannel)client;

            channel.Open();
            beginAction(client, request, r =>
            {
                try
                {
                    endAction(client);
                }
                catch
                {
                    return;
                }
                finally
                {
                    if (channel != null)
                    {
                        try
                        {
                            channel.Close();
                        }
                        catch
                        {
                            channel.Abort();
                        }
                    }
                }

                callback();
            }
                , null);
        }

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that returns a response.
        /// </summary>
        /// <typeparam name="TResponse">Type of response used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        public void UseAsync<TResponse>(Func<TService, AsyncCallback, object, IAsyncResult> beginAction,
            Func<TService, IAsyncResult, TResponse> endAction, Action<TResponse> callback)
        {
            var client = this._channelFactory.CreateChannel();
            var channel = (IClientChannel)client;

            channel.Open();
            beginAction(client, r =>
            {
                TResponse response;
                try
                {
                    response = endAction(client, r);
                }
                catch
                {
                    return;
                }
                finally
                {
                    if (channel != null)
                    {
                        try
                        {
                            channel.Close();
                        }
                        catch
                        {
                            channel.Abort();
                        }
                    }
                }

                callback(response);
            }
                , null);
        }

        /// <summary>
        ///     Performs an asyncrhonous operation with the service proxy that requires a request parameter and returns a response.
        /// </summary>
        /// <typeparam name="TRequest">Type of request used by the operation.</typeparam>
        /// <typeparam name="TResponse">Type of response used by the operation.</typeparam>
        /// <param name="beginAction">Function delegate to perform for beginning the asynchronous operation.</param>
        /// <param name="request">Request to pass to the beginning action delegate.</param>
        /// <param name="endAction">Function delegate to perform for ending the asynchronous operation.</param>
        /// <param name="callback">Function delegate to perform for indicating the end of the asynchronous operation.</param>
        /// <remarks>
        ///     This method provides a best-practice pattern for utilizing WCF for web service method calls.
        ///     Invoking service methods using this method ensures a client channel is properly closed after the desired
        ///     action has completed.  It also ensures the channel is aborted properly in the event of an unhandled
        ///     exception.
        /// </remarks>
        public void UseAsync<TRequest, TResponse>(
            Func<TService, TRequest, AsyncCallback, object, IAsyncResult> beginAction, TRequest request,
            Func<TService, IAsyncResult, TResponse> endAction, Action<TResponse> callback)
        {
            var client = this._channelFactory.CreateChannel();
            var channel = (IClientChannel)client;

            channel.Open();
            beginAction(client, request, r =>
            {
                TResponse response;
                try
                {
                    response = endAction(client, r);
                }
                catch
                {
                    return;
                }
                finally
                {
                    if (channel != null)
                    {
                        try
                        {
                            channel.Close();
                        }
                        catch
                        {
                            channel.Abort();
                        }
                    }
                }

                callback(response);
            }
                , null);
        }
    }
}