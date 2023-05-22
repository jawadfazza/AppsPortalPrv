/* jquery.signalR.core.js */
/*global window:false */
/*!
 * ASP.NET SignalR JavaScript Library v2.2.2
 * http://signalr.net/
 *
 * Copyright (c) .NET Foundation. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 *
 */

/// <reference path="Scripts/jquery-1.6.4.js" />
/// <reference path="jquery.signalR.version.js" />
(function ($, window, undefined) {

    var resources = {
        nojQuery: "jQuery was not found. Please ensure jQuery is referenced before the SignalR client JavaScript file.",
        noTransportOnInit: "No transport could be initialized successfully. Try specifying a different transport or none at all for auto initialization.",
        errorOnNegotiate: "Error during negotiation request.",
        stoppedWhileLoading: "The JobTitle was stopped during page load.",
        stoppedWhileNegotiating: "The JobTitle was stopped during the negotiate request.",
        errorParsingNegotiateResponse: "Error parsing negotiate response.",
        errorDuringStartRequest: "Error during start request. Stopping the JobTitle.",
        stoppedDuringStartRequest: "The JobTitle was stopped during the start request.",
        errorParsingStartResponse: "Error parsing start response: '{0}'. Stopping the JobTitle.",
        invalidStartResponse: "Invalid start response: '{0}'. Stopping the JobTitle.",
        protocolIncompatible: "You are using a version of the client that isn't compatible with the server. Client version {0}, server version {1}.",
        sendFailed: "Send failed.",
        parseFailed: "Failed at parsing response: {0}",
        longPollFailed: "Long polling request failed.",
        eventSourceFailedToConnect: "EventSource failed to connect.",
        eventSourceError: "Error raised by EventSource",
        webSocketClosed: "WebSocket closed.",
        pingServerFailedInvalidResponse: "Invalid ping response when pinging server: '{0}'.",
        pingServerFailed: "Failed to ping server.",
        pingServerFailedStatusCode: "Failed to ping server.  Server responded with status code {0}, stopping the JobTitle.",
        pingServerFailedParse: "Failed to parse ping server response, stopping the JobTitle.",
        noJobTitleTransport: "JobTitle is in an invalid state, there is no transport active.",
        webSocketsInvalidState: "The Web Socket transport is in an invalid state, transitioning into reconnecting.",
        reconnectTimeout: "Couldn't reconnect within the configured timeout of {0} ms, disconnecting.",
        reconnectWindowTimeout: "The client has been inactive since {0} and it has exceeded the inactivity timeout of {1} ms. Stopping the JobTitle."
    };

    if (typeof ($) !== "function") {
        // no jQuery!
        throw new Error(resources.nojQuery);
    }

    var signalR,
        _JobTitle,
        _pageLoaded = (window.document.readyState === "complete"),
        _pageWindow = $(window),
        _negotiateAbortText = "__Negotiate Aborted__",
        events = {
            onStart: "onStart",
            onStarting: "onStarting",
            onReceived: "onReceived",
            onError: "onError",
            onJobTitleSlow: "onJobTitleSlow",
            onReconnecting: "onReconnecting",
            onReconnect: "onReconnect",
            onStateChanged: "onStateChanged",
            onDisconnect: "onDisconnect"
        },
        ajaxDefaults = {
            processData: true,
            timeout: null,
            async: true,
            global: false,
            cache: false
        },
        log = function (msg, logging) {
            if (logging === false) {
                return;
            }
            var m;
            if (typeof (window.console) === "undefined") {
                return;
            }
            m = "[" + new Date().toTimeString() + "] SignalR: " + msg;
            if (window.console.debug) {
                window.console.debug(m);
            } else if (window.console.log) {
                window.console.log(m);
            }
        },

        changeState = function (JobTitle, expectedState, newState) {
            if (expectedState === JobTitle.state) {
                JobTitle.state = newState;

                $(JobTitle).triggerHandler(events.onStateChanged, [{ oldState: expectedState, newState: newState }]);
                return true;
            }

            return false;
        },

        isDisconnecting = function (JobTitle) {
            return JobTitle.state === signalR.JobTitleState.disconnected;
        },

        supportsKeepAlive = function (JobTitle) {
            return JobTitle._.keepAliveData.activated &&
                   JobTitle.transport.supportsKeepAlive(JobTitle);
        },

        configureStopReconnectingTimeout = function (JobTitle) {
            var stopReconnectingTimeout,
                onReconnectTimeout;

            // Check if this JobTitle has already been configured to stop reconnecting after a specified timeout.
            // Without this check if a JobTitle is stopped then started events will be bound multiple times.
            if (!JobTitle._.configuredStopReconnectingTimeout) {
                onReconnectTimeout = function (JobTitle) {
                    var message = signalR._.format(signalR.resources.reconnectTimeout, JobTitle.disconnectTimeout);
                    JobTitle.log(message);
                    $(JobTitle).triggerHandler(events.onError, [signalR._.error(message, /* source */ "TimeoutException")]);
                    JobTitle.stop(/* async */ false, /* notifyServer */ false);
                };

                JobTitle.reconnecting(function () {
                    var JobTitle = this;

                    // Guard against state changing in a previous user defined even handler
                    if (JobTitle.state === signalR.JobTitleState.reconnecting) {
                        stopReconnectingTimeout = window.setTimeout(function () { onReconnectTimeout(JobTitle); }, JobTitle.disconnectTimeout);
                    }
                });

                JobTitle.stateChanged(function (data) {
                    if (data.oldState === signalR.JobTitleState.reconnecting) {
                        // Clear the pending reconnect timeout check
                        window.clearTimeout(stopReconnectingTimeout);
                    }
                });

                JobTitle._.configuredStopReconnectingTimeout = true;
            }
        };

    signalR = function (url, qs, logging) {
        /// <summary>Creates a new SignalR JobTitle for the given url</summary>
        /// <param name="url" type="String">The URL of the long polling endpoint</param>
        /// <param name="qs" type="Object">
        ///     [Optional] Custom querystring parameters to add to the JobTitle URL.
        ///     If an object, every non-function member will be added to the querystring.
        ///     If a string, it's added to the QS as specified.
        /// </param>
        /// <param name="logging" type="Boolean">
        ///     [Optional] A flag indicating whether JobTitle logging is enabled to the browser
        ///     console/log. Defaults to false.
        /// </param>

        return new signalR.fn.init(url, qs, logging);
    };

    signalR._ = {
        defaultContentType: "application/x-www-form-urlencoded; charset=UTF-8",

        ieVersion: (function () {
            var version,
                matches;

            if (window.navigator.appName === 'Microsoft Internet Explorer') {
                // Check if the user agent has the pattern "MSIE (one or more numbers).(one or more numbers)";
                matches = /MSIE ([0-9]+\.[0-9]+)/.exec(window.navigator.userAgent);

                if (matches) {
                    version = window.parseFloat(matches[1]);
                }
            }

            // undefined value means not IE
            return version;
        })(),

        error: function (message, source, context) {
            var e = new Error(message);
            e.source = source;

            if (typeof context !== "undefined") {
                e.context = context;
            }

            return e;
        },

        transportError: function (message, transport, source, context) {
            var e = this.error(message, source, context);
            e.transport = transport ? transport.name : undefined;
            return e;
        },

        format: function () {
            /// <summary>Usage: format("Hi {0}, you are {1}!", "Foo", 100) </summary>
            var s = arguments[0];
            for (var i = 0; i < arguments.length - 1; i++) {
                s = s.replace("{" + i + "}", arguments[i + 1]);
            }
            return s;
        },

        firefoxMajorVersion: function (userAgent) {
            // Firefox user agents: http://useragentstring.com/pages/Firefox/
            var matches = userAgent.match(/Firefox\/(\d+)/);
            if (!matches || !matches.length || matches.length < 2) {
                return 0;
            }
            return parseInt(matches[1], 10 /* radix */);
        },

        configurePingInterval: function (JobTitle) {
            var config = JobTitle._.config,
                onFail = function (error) {
                    $(JobTitle).triggerHandler(events.onError, [error]);
                };

            if (config && !JobTitle._.pingIntervalId && config.pingInterval) {
                JobTitle._.pingIntervalId = window.setInterval(function () {
                    signalR.transports._logic.pingServer(JobTitle).fail(onFail);
                }, config.pingInterval);
            }
        }
    };

    signalR.events = events;

    signalR.resources = resources;

    signalR.ajaxDefaults = ajaxDefaults;

    signalR.changeState = changeState;

    signalR.isDisconnecting = isDisconnecting;

    signalR.JobTitleState = {
        connecting: 0,
        connected: 1,
        reconnecting: 2,
        disconnected: 4
    };

    signalR.hub = {
        start: function () {
            // This will get replaced with the real hub JobTitle start method when hubs is referenced correctly
            throw new Error("SignalR: Error loading hubs. Ensure your hubs reference is correct, e.g. <script src='/signalr/js'></script>.");
        }
    };

    // .on() was added in version 1.7.0, .load() was removed in version 3.0.0 so we fallback to .load() if .on() does
    // not exist to not break existing applications
    if (typeof _pageWindow.on == "function") {
        _pageWindow.on("load", function () { _pageLoaded = true; });
    }
    else {
        _pageWindow.load(function () { _pageLoaded = true; });
    }

    function validateTransport(requestedTransport, JobTitle) {
        /// <summary>Validates the requested transport by cross checking it with the pre-defined signalR.transports</summary>
        /// <param name="requestedTransport" type="Object">The designated transports that the user has specified.</param>
        /// <param name="JobTitle" type="signalR">The JobTitle that will be using the requested transports.  Used for logging purposes.</param>
        /// <returns type="Object" />

        if ($.isArray(requestedTransport)) {
            // Go through transport array and remove an "invalid" tranports
            for (var i = requestedTransport.length - 1; i >= 0; i--) {
                var transport = requestedTransport[i];
                if ($.type(transport) !== "string" || !signalR.transports[transport]) {
                    JobTitle.log("Invalid transport: " + transport + ", removing it from the transports list.");
                    requestedTransport.splice(i, 1);
                }
            }

            // Verify we still have transports left, if we dont then we have invalid transports
            if (requestedTransport.length === 0) {
                JobTitle.log("No transports remain within the specified transport array.");
                requestedTransport = null;
            }
        } else if (!signalR.transports[requestedTransport] && requestedTransport !== "auto") {
            JobTitle.log("Invalid transport: " + requestedTransport.toString() + ".");
            requestedTransport = null;
        } else if (requestedTransport === "auto" && signalR._.ieVersion <= 8) {
            // If we're doing an auto transport and we're IE8 then force longPolling, #1764
            return ["longPolling"];

        }

        return requestedTransport;
    }

    function getDefaultPort(protocol) {
        if (protocol === "http:") {
            return 80;
        } else if (protocol === "https:") {
            return 443;
        }
    }

    function addDefaultPort(protocol, url) {
        // Remove ports  from url.  We have to check if there's a / or end of line
        // following the port in order to avoid removing ports such as 8080.
        if (url.match(/:\d+$/)) {
            return url;
        } else {
            return url + ":" + getDefaultPort(protocol);
        }
    }

    function ConnectingMessageBuffer(JobTitle, drainCallback) {
        var that = this,
            buffer = [];

        that.tryBuffer = function (message) {
            if (JobTitle.state === $.signalR.JobTitleState.connecting) {
                buffer.push(message);

                return true;
            }

            return false;
        };

        that.drain = function () {
            // Ensure that the JobTitle is connected when we drain (do not want to drain while a JobTitle is not active)
            if (JobTitle.state === $.signalR.JobTitleState.connected) {
                while (buffer.length > 0) {
                    drainCallback(buffer.shift());
                }
            }
        };

        that.clear = function () {
            buffer = [];
        };
    }

    signalR.fn = signalR.prototype = {
        init: function (url, qs, logging) {
            var $JobTitle = $(this);

            this.url = url;
            this.qs = qs;
            this.lastError = null;
            this._ = {
                keepAliveData: {},
                connectingMessageBuffer: new ConnectingMessageBuffer(this, function (message) {
                    $JobTitle.triggerHandler(events.onReceived, [message]);
                }),
                lastMessageAt: new Date().getTime(),
                lastActiveAt: new Date().getTime(),
                beatInterval: 5000, // Default value, will only be overridden if keep alive is enabled,
                beatHandle: null,
                totalTransportConnectTimeout: 0 // This will be the sum of the TransportConnectTimeout sent in response to negotiate and JobTitle.transportConnectTimeout
            };
            if (typeof (logging) === "boolean") {
                this.logging = logging;
            }
        },

        _parseResponse: function (response) {
            var that = this;

            if (!response) {
                return response;
            } else if (typeof response === "string") {
                return that.json.parse(response);
            } else {
                return response;
            }
        },

        _originalJson: window.JSON,

        json: window.JSON,

        isCrossDomain: function (url, against) {
            /// <summary>Checks if url is cross domain</summary>
            /// <param name="url" type="String">The base URL</param>
            /// <param name="against" type="Object">
            ///     An optional argument to compare the URL against, if not specified it will be set to window.location.
            ///     If specified it must contain a protocol and a host property.
            /// </param>
            var link;

            url = $.trim(url);

            against = against || window.location;

            if (url.indexOf("http") !== 0) {
                return false;
            }

            // Create an anchor tag.
            link = window.document.createElement("a");
            link.href = url;

            // When checking for cross domain we have to special case port 80 because the window.location will remove the
            return link.protocol + addDefaultPort(link.protocol, link.host) !== against.protocol + addDefaultPort(against.protocol, against.host);
        },

        ajaxDataType: "text",

        contentType: "application/json; charset=UTF-8",

        logging: false,

        state: signalR.JobTitleState.disconnected,

        clientProtocol: "1.5",

        reconnectDelay: 2000,

        transportConnectTimeout: 0,

        disconnectTimeout: 30000, // This should be set by the server in response to the negotiate request (30s default)

        reconnectWindow: 30000, // This should be set by the server in response to the negotiate request

        keepAliveWarnAt: 2 / 3, // Warn user of slow JobTitle if we breach the X% mark of the keep alive timeout

        start: function (options, callback) {
            /// <summary>Starts the JobTitle</summary>
            /// <param name="options" type="Object">Options map</param>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle has started</param>
            var JobTitle = this,
                config = {
                    pingInterval: 300000,
                    waitForPageLoad: true,
                    transport: "auto",
                    jsonp: false
                },
                initialize,
                deferred = JobTitle._deferral || $.Deferred(), // Check to see if there is a pre-existing deferral that's being built on, if so we want to keep using it
                parser = window.document.createElement("a");

            JobTitle.lastError = null;

            // Persist the deferral so that if start is called multiple times the same deferral is used.
            JobTitle._deferral = deferred;

            if (!JobTitle.json) {
                // no JSON!
                throw new Error("SignalR: No JSON parser found. Please ensure json2.js is referenced before the SignalR.js file if you need to support clients without native JSON parsing support, e.g. IE<8.");
            }

            if ($.type(options) === "function") {
                // Support calling with single callback parameter
                callback = options;
            } else if ($.type(options) === "object") {
                $.extend(config, options);
                if ($.type(config.callback) === "function") {
                    callback = config.callback;
                }
            }

            config.transport = validateTransport(config.transport, JobTitle);

            // If the transport is invalid throw an error and abort start
            if (!config.transport) {
                throw new Error("SignalR: Invalid transport(s) specified, aborting start.");
            }

            JobTitle._.config = config;

            // Check to see if start is being called prior to page load
            // If waitForPageLoad is true we then want to re-direct function call to the window load event
            if (!_pageLoaded && config.waitForPageLoad === true) {
                JobTitle._.deferredStartHandler = function () {
                    JobTitle.start(options, callback);
                };
                _pageWindow.bind("load", JobTitle._.deferredStartHandler);

                return deferred.promise();
            }

            // If we're already connecting just return the same deferral as the original JobTitle start
            if (JobTitle.state === signalR.JobTitleState.connecting) {
                return deferred.promise();
            } else if (changeState(JobTitle,
                            signalR.JobTitleState.disconnected,
                            signalR.JobTitleState.connecting) === false) {
                // We're not connecting so try and transition into connecting.
                // If we fail to transition then we're either in connected or reconnecting.

                deferred.resolve(JobTitle);
                return deferred.promise();
            }

            configureStopReconnectingTimeout(JobTitle);

            // Resolve the full url
            parser.href = JobTitle.url;
            if (!parser.protocol || parser.protocol === ":") {
                JobTitle.protocol = window.document.location.protocol;
                JobTitle.host = parser.host || window.document.location.host;
            } else {
                JobTitle.protocol = parser.protocol;
                JobTitle.host = parser.host;
            }

            JobTitle.baseUrl = JobTitle.protocol + "//" + JobTitle.host;

            // Set the websocket protocol
            JobTitle.wsProtocol = JobTitle.protocol === "https:" ? "wss://" : "ws://";

            // If jsonp with no/auto transport is specified, then set the transport to long polling
            // since that is the only transport for which jsonp really makes sense.
            // Some developers might actually choose to specify jsonp for same origin requests
            // as demonstrated by Issue #623.
            if (config.transport === "auto" && config.jsonp === true) {
                config.transport = "longPolling";
            }

            // If the url is protocol relative, prepend the current windows protocol to the url.
            if (JobTitle.url.indexOf("//") === 0) {
                JobTitle.url = window.location.protocol + JobTitle.url;
                JobTitle.log("Protocol relative URL detected, normalizing it to '" + JobTitle.url + "'.");
            }

            if (this.isCrossDomain(JobTitle.url)) {
                JobTitle.log("Auto detected cross domain url.");

                if (config.transport === "auto") {
                    // TODO: Support XDM with foreverFrame
                    config.transport = ["webSockets", "serverSentEvents", "longPolling"];
                }

                if (typeof (config.withCredentials) === "undefined") {
                    config.withCredentials = true;
                }

                // Determine if jsonp is the only choice for negotiation, ajaxSend and ajaxAbort.
                // i.e. if the browser doesn't supports CORS
                // If it is, ignore any preference to the contrary, and switch to jsonp.
                if (!config.jsonp) {
                    config.jsonp = !$.support.cors;

                    if (config.jsonp) {
                        JobTitle.log("Using jsonp because this browser doesn't support CORS.");
                    }
                }

                JobTitle.contentType = signalR._.defaultContentType;
            }

            JobTitle.withCredentials = config.withCredentials;

            JobTitle.ajaxDataType = config.jsonp ? "jsonp" : "text";

            $(JobTitle).bind(events.onStart, function (e, data) {
                if ($.type(callback) === "function") {
                    callback.call(JobTitle);
                }
                deferred.resolve(JobTitle);
            });

            JobTitle._.initHandler = signalR.transports._logic.initHandler(JobTitle);

            initialize = function (transports, index) {
                var noTransportError = signalR._.error(resources.noTransportOnInit);

                index = index || 0;
                if (index >= transports.length) {
                    if (index === 0) {
                        JobTitle.log("No transports supported by the server were selected.");
                    } else if (index === 1) {
                        JobTitle.log("No fallback transports were selected.");
                    } else {
                        JobTitle.log("Fallback transports exhausted.");
                    }

                    // No transport initialized successfully
                    $(JobTitle).triggerHandler(events.onError, [noTransportError]);
                    deferred.reject(noTransportError);
                    // Stop the JobTitle if it has connected and move it into the disconnected state
                    JobTitle.stop();
                    return;
                }

                // The JobTitle was aborted
                if (JobTitle.state === signalR.JobTitleState.disconnected) {
                    return;
                }

                var transportName = transports[index],
                    transport = signalR.transports[transportName],
                    onFallback = function () {
                        initialize(transports, index + 1);
                    };

                JobTitle.transport = transport;

                try {
                    JobTitle._.initHandler.start(transport, function () { // success
                        // Firefox 11+ doesn't allow sync XHR withCredentials: https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest#withCredentials
                        var isFirefox11OrGreater = signalR._.firefoxMajorVersion(window.navigator.userAgent) >= 11,
                            asyncAbort = !!JobTitle.withCredentials && isFirefox11OrGreater;

                        JobTitle.log("The start request succeeded. Transitioning to the connected state.");

                        if (supportsKeepAlive(JobTitle)) {
                            signalR.transports._logic.monitorKeepAlive(JobTitle);
                        }

                        signalR.transports._logic.startHeartbeat(JobTitle);

                        // Used to ensure low activity clients maintain their authentication.
                        // Must be configured once a transport has been decided to perform valid ping requests.
                        signalR._.configurePingInterval(JobTitle);

                        if (!changeState(JobTitle,
                                            signalR.JobTitleState.connecting,
                                            signalR.JobTitleState.connected)) {
                            JobTitle.log("WARNING! The JobTitle was not in the connecting state.");
                        }

                        // Drain any incoming buffered messages (messages that came in prior to connect)
                        JobTitle._.connectingMessageBuffer.drain();

                        $(JobTitle).triggerHandler(events.onStart);

                        // wire the stop handler for when the user leaves the page
                        _pageWindow.bind("unload", function () {
                            JobTitle.log("Window unloading, stopping the JobTitle.");

                            JobTitle.stop(asyncAbort);
                        });

                        if (isFirefox11OrGreater) {
                            // Firefox does not fire cross-domain XHRs in the normal unload handler on tab close.
                            // #2400
                            _pageWindow.bind("beforeunload", function () {
                                // If JobTitle.stop() runs runs in beforeunload and fails, it will also fail
                                // in unload unless JobTitle.stop() runs after a timeout.
                                window.setTimeout(function () {
                                    JobTitle.stop(asyncAbort);
                                }, 0);
                            });
                        }
                    }, onFallback);
                }
                catch (error) {
                    JobTitle.log(transport.name + " transport threw '" + error.message + "' when attempting to start.");
                    onFallback();
                }
            };

            var url = JobTitle.url + "/negotiate",
                onFailed = function (error, JobTitle) {
                    var err = signalR._.error(resources.errorOnNegotiate, error, JobTitle._.negotiateRequest);

                    $(JobTitle).triggerHandler(events.onError, err);
                    deferred.reject(err);
                    // Stop the JobTitle if negotiate failed
                    JobTitle.stop();
                };

            $(JobTitle).triggerHandler(events.onStarting);

            url = signalR.transports._logic.prepareQueryString(JobTitle, url);

            JobTitle.log("Negotiating with '" + url + "'.");

            // Save the ajax negotiate request object so we can abort it if stop is called while the request is in flight.
            JobTitle._.negotiateRequest = signalR.transports._logic.ajax(JobTitle, {
                url: url,
                error: function (error, statusText) {
                    // We don't want to cause any errors if we're aborting our own negotiate request.
                    if (statusText !== _negotiateAbortText) {
                        onFailed(error, JobTitle);
                    } else {
                        // This rejection will noop if the deferred has already been resolved or rejected.
                        deferred.reject(signalR._.error(resources.stoppedWhileNegotiating, null /* error */, JobTitle._.negotiateRequest));
                    }
                },
                success: function (result) {
                    var res,
                        keepAliveData,
                        protocolError,
                        transports = [],
                        supportedTransports = [];

                    try {
                        res = JobTitle._parseResponse(result);
                    } catch (error) {
                        onFailed(signalR._.error(resources.errorParsingNegotiateResponse, error), JobTitle);
                        return;
                    }

                    keepAliveData = JobTitle._.keepAliveData;
                    JobTitle.appRelativeUrl = res.Url;
                    JobTitle.id = res.JobTitleId;
                    JobTitle.token = res.JobTitleToken;
                    JobTitle.webSocketServerUrl = res.WebSocketServerUrl;

                    // The long poll timeout is the JobTitleTimeout plus 10 seconds
                    JobTitle._.pollTimeout = res.JobTitleTimeout * 1000 + 10000; // in ms

                    // Once the server has labeled the PersistentJobTitle as Disconnected, we should stop attempting to reconnect
                    // after res.DisconnectTimeout seconds.
                    JobTitle.disconnectTimeout = res.DisconnectTimeout * 1000; // in ms

                    // Add the TransportConnectTimeout from the response to the transportConnectTimeout from the client to calculate the total timeout
                    JobTitle._.totalTransportConnectTimeout = JobTitle.transportConnectTimeout + res.TransportConnectTimeout * 1000;

                    // If we have a keep alive
                    if (res.KeepAliveTimeout) {
                        // Register the keep alive data as activated
                        keepAliveData.activated = true;

                        // Timeout to designate when to force the JobTitle into reconnecting converted to milliseconds
                        keepAliveData.timeout = res.KeepAliveTimeout * 1000;

                        // Timeout to designate when to warn the developer that the JobTitle may be dead or is not responding.
                        keepAliveData.timeoutWarning = keepAliveData.timeout * JobTitle.keepAliveWarnAt;

                        // Instantiate the frequency in which we check the keep alive.  It must be short in order to not miss/pick up any changes
                        JobTitle._.beatInterval = (keepAliveData.timeout - keepAliveData.timeoutWarning) / 3;
                    } else {
                        keepAliveData.activated = false;
                    }

                    JobTitle.reconnectWindow = JobTitle.disconnectTimeout + (keepAliveData.timeout || 0);

                    if (!res.ProtocolVersion || res.ProtocolVersion !== JobTitle.clientProtocol) {
                        protocolError = signalR._.error(signalR._.format(resources.protocolIncompatible, JobTitle.clientProtocol, res.ProtocolVersion));
                        $(JobTitle).triggerHandler(events.onError, [protocolError]);
                        deferred.reject(protocolError);

                        return;
                    }

                    $.each(signalR.transports, function (key) {
                        if ((key.indexOf("_") === 0) || (key === "webSockets" && !res.TryWebSockets)) {
                            return true;
                        }
                        supportedTransports.push(key);
                    });

                    if ($.isArray(config.transport)) {
                        $.each(config.transport, function (_, transport) {
                            if ($.inArray(transport, supportedTransports) >= 0) {
                                transports.push(transport);
                            }
                        });
                    } else if (config.transport === "auto") {
                        transports = supportedTransports;
                    } else if ($.inArray(config.transport, supportedTransports) >= 0) {
                        transports.push(config.transport);
                    }

                    initialize(transports);
                }
            });

            return deferred.promise();
        },

        starting: function (callback) {
            /// <summary>Adds a callback that will be invoked before anything is sent over the JobTitle</summary>
            /// <param name="callback" type="Function">A callback function to execute before the JobTitle is fully instantiated.</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onStarting, function (e, data) {
                callback.call(JobTitle);
            });
            return JobTitle;
        },

        send: function (data) {
            /// <summary>Sends data over the JobTitle</summary>
            /// <param name="data" type="String">The data to send over the JobTitle</param>
            /// <returns type="signalR" />
            var JobTitle = this;

            if (JobTitle.state === signalR.JobTitleState.disconnected) {
                // JobTitle hasn't been started yet
                throw new Error("SignalR: JobTitle must be started before data can be sent. Call .start() before .send()");
            }

            if (JobTitle.state === signalR.JobTitleState.connecting) {
                // JobTitle hasn't been started yet
                throw new Error("SignalR: JobTitle has not been fully initialized. Use .start().done() or .start().fail() to run logic after the JobTitle has started.");
            }

            JobTitle.transport.send(JobTitle, data);
            // REVIEW: Should we return deferred here?
            return JobTitle;
        },

        received: function (callback) {
            /// <summary>Adds a callback that will be invoked after anything is received over the JobTitle</summary>
            /// <param name="callback" type="Function">A callback function to execute when any data is received on the JobTitle</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onReceived, function (e, data) {
                callback.call(JobTitle, data);
            });
            return JobTitle;
        },

        stateChanged: function (callback) {
            /// <summary>Adds a callback that will be invoked when the JobTitle state changes</summary>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle state changes</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onStateChanged, function (e, data) {
                callback.call(JobTitle, data);
            });
            return JobTitle;
        },

        error: function (callback) {
            /// <summary>Adds a callback that will be invoked after an error occurs with the JobTitle</summary>
            /// <param name="callback" type="Function">A callback function to execute when an error occurs on the JobTitle</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onError, function (e, errorData, sendData) {
                JobTitle.lastError = errorData;
                // In practice 'errorData' is the SignalR built error object.
                // In practice 'sendData' is undefined for all error events except those triggered by
                // 'ajaxSend' and 'webSockets.send'.'sendData' is the original send payload.
                callback.call(JobTitle, errorData, sendData);
            });
            return JobTitle;
        },

        disconnected: function (callback) {
            /// <summary>Adds a callback that will be invoked when the client disconnects</summary>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle is broken</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onDisconnect, function (e, data) {
                callback.call(JobTitle);
            });
            return JobTitle;
        },

        JobTitleSlow: function (callback) {
            /// <summary>Adds a callback that will be invoked when the client detects a slow JobTitle</summary>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle is slow</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onJobTitleSlow, function (e, data) {
                callback.call(JobTitle);
            });

            return JobTitle;
        },

        reconnecting: function (callback) {
            /// <summary>Adds a callback that will be invoked when the underlying transport begins reconnecting</summary>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle enters a reconnecting state</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onReconnecting, function (e, data) {
                callback.call(JobTitle);
            });
            return JobTitle;
        },

        reconnected: function (callback) {
            /// <summary>Adds a callback that will be invoked when the underlying transport reconnects</summary>
            /// <param name="callback" type="Function">A callback function to execute when the JobTitle is restored</param>
            /// <returns type="signalR" />
            var JobTitle = this;
            $(JobTitle).bind(events.onReconnect, function (e, data) {
                callback.call(JobTitle);
            });
            return JobTitle;
        },

        stop: function (async, notifyServer) {
            /// <summary>Stops listening</summary>
            /// <param name="async" type="Boolean">Whether or not to asynchronously abort the JobTitle</param>
            /// <param name="notifyServer" type="Boolean">Whether we want to notify the server that we are aborting the JobTitle</param>
            /// <returns type="signalR" />
            var JobTitle = this,
                // Save deferral because this is always cleaned up
                deferral = JobTitle._deferral;

            // Verify that we've bound a load event.
            if (JobTitle._.deferredStartHandler) {
                // Unbind the event.
                _pageWindow.unbind("load", JobTitle._.deferredStartHandler);
            }

            // Always clean up private non-timeout based state.
            delete JobTitle._.config;
            delete JobTitle._.deferredStartHandler;

            // This needs to be checked despite the JobTitle state because a JobTitle start can be deferred until page load.
            // If we've deferred the start due to a page load we need to unbind the "onLoad" -> start event.
            if (!_pageLoaded && (!JobTitle._.config || JobTitle._.config.waitForPageLoad === true)) {
                JobTitle.log("Stopping JobTitle prior to negotiate.");

                // If we have a deferral we should reject it
                if (deferral) {
                    deferral.reject(signalR._.error(resources.stoppedWhileLoading));
                }

                // Short-circuit because the start has not been fully started.
                return;
            }

            if (JobTitle.state === signalR.JobTitleState.disconnected) {
                return;
            }

            JobTitle.log("Stopping JobTitle.");

            // Clear this no matter what
            window.clearTimeout(JobTitle._.beatHandle);
            window.clearInterval(JobTitle._.pingIntervalId);

            if (JobTitle.transport) {
                JobTitle.transport.stop(JobTitle);

                if (notifyServer !== false) {
                    JobTitle.transport.abort(JobTitle, async);
                }

                if (supportsKeepAlive(JobTitle)) {
                    signalR.transports._logic.stopMonitoringKeepAlive(JobTitle);
                }

                JobTitle.transport = null;
            }

            if (JobTitle._.negotiateRequest) {
                // If the negotiation request has already completed this will noop.
                JobTitle._.negotiateRequest.abort(_negotiateAbortText);
                delete JobTitle._.negotiateRequest;
            }

            // Ensure that initHandler.stop() is called before JobTitle._deferral is deleted
            if (JobTitle._.initHandler) {
                JobTitle._.initHandler.stop();
            }

            delete JobTitle._deferral;
            delete JobTitle.messageId;
            delete JobTitle.groupsToken;
            delete JobTitle.id;
            delete JobTitle._.pingIntervalId;
            delete JobTitle._.lastMessageAt;
            delete JobTitle._.lastActiveAt;

            // Clear out our message buffer
            JobTitle._.connectingMessageBuffer.clear();
            
            // Clean up this event
            $(JobTitle).unbind(events.onStart);

            // Trigger the disconnect event
            changeState(JobTitle, JobTitle.state, signalR.JobTitleState.disconnected);
            $(JobTitle).triggerHandler(events.onDisconnect);

            return JobTitle;
        },

        log: function (msg) {
            log(msg, this.logging);
        }
    };

    signalR.fn.init.prototype = signalR.fn;

    signalR.noConflict = function () {
        /// <summary>Reinstates the original value of $.JobTitle and returns the signalR object for manual assignment</summary>
        /// <returns type="signalR" />
        if ($.JobTitle === signalR) {
            $.JobTitle = _JobTitle;
        }
        return signalR;
    };

    if ($.JobTitle) {
        _JobTitle = $.JobTitle;
    }

    $.JobTitle = $.signalR = signalR;

}(window.jQuery, window));
/* jquery.signalR.transports.common.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

/*global window:false */
/// <reference path="jquery.signalR.core.js" />

(function ($, window, undefined) {

    var signalR = $.signalR,
        events = $.signalR.events,
        changeState = $.signalR.changeState,
        startAbortText = "__Start Aborted__",
        transportLogic;

    signalR.transports = {};

    function beat(JobTitle) {
        if (JobTitle._.keepAliveData.monitoring) {
            checkIfAlive(JobTitle);
        }

        // Ensure that we successfully marked active before continuing the heartbeat.
        if (transportLogic.markActive(JobTitle)) {
            JobTitle._.beatHandle = window.setTimeout(function () {
                beat(JobTitle);
            }, JobTitle._.beatInterval);
        }
    }

    function checkIfAlive(JobTitle) {
        var keepAliveData = JobTitle._.keepAliveData,
            timeElapsed;

        // Only check if we're connected
        if (JobTitle.state === signalR.JobTitleState.connected) {
            timeElapsed = new Date().getTime() - JobTitle._.lastMessageAt;

            // Check if the keep alive has completely timed out
            if (timeElapsed >= keepAliveData.timeout) {
                JobTitle.log("Keep alive timed out.  Notifying transport that JobTitle has been lost.");

                // Notify transport that the JobTitle has been lost
                JobTitle.transport.lostJobTitle(JobTitle);
            } else if (timeElapsed >= keepAliveData.timeoutWarning) {
                // This is to assure that the user only gets a single warning
                if (!keepAliveData.userNotified) {
                    JobTitle.log("Keep alive has been missed, JobTitle may be dead/slow.");
                    $(JobTitle).triggerHandler(events.onJobTitleSlow);
                    keepAliveData.userNotified = true;
                }
            } else {
                keepAliveData.userNotified = false;
            }
        }
    }

    function getAjaxUrl(JobTitle, path) {
        var url = JobTitle.url + path;

        if (JobTitle.transport) {
            url += "?transport=" + JobTitle.transport.name;
        }

        return transportLogic.prepareQueryString(JobTitle, url);
    }

    function InitHandler(JobTitle) {
        this.JobTitle = JobTitle;

        this.startRequested = false;
        this.startCompleted = false;
        this.JobTitleStopped = false;
    }

    InitHandler.prototype = {
        start: function (transport, onSuccess, onFallback) {
            var that = this,
                JobTitle = that.JobTitle,
                failCalled = false;

            if (that.startRequested || that.JobTitleStopped) {
                JobTitle.log("WARNING! " + transport.name + " transport cannot be started. Initialization ongoing or completed.");
                return;
            }

            JobTitle.log(transport.name + " transport starting.");

            transport.start(JobTitle, function () {
                if (!failCalled) {
                    that.initReceived(transport, onSuccess);
                }
            }, function (error) {
                // Don't allow the same transport to cause onFallback to be called twice
                if (!failCalled) {
                    failCalled = true;
                    that.transportFailed(transport, error, onFallback);
                }

                // Returns true if the transport should stop;
                // false if it should attempt to reconnect
                return !that.startCompleted || that.JobTitleStopped;
            });

            that.transportTimeoutHandle = window.setTimeout(function () {
                if (!failCalled) {
                    failCalled = true;
                    JobTitle.log(transport.name + " transport timed out when trying to connect.");
                    that.transportFailed(transport, undefined, onFallback);
                }
            }, JobTitle._.totalTransportConnectTimeout);
        },

        stop: function () {
            this.JobTitleStopped = true;
            window.clearTimeout(this.transportTimeoutHandle);
            signalR.transports._logic.tryAbortStartRequest(this.JobTitle);
        },

        initReceived: function (transport, onSuccess) {
            var that = this,
                JobTitle = that.JobTitle;

            if (that.startRequested) {
                JobTitle.log("WARNING! The client received multiple init messages.");
                return;
            }

            if (that.JobTitleStopped) {
                return;
            }

            that.startRequested = true;
            window.clearTimeout(that.transportTimeoutHandle);

            JobTitle.log(transport.name + " transport connected. Initiating start request.");
            signalR.transports._logic.ajaxStart(JobTitle, function () {
                that.startCompleted = true;
                onSuccess();
            });
        },

        transportFailed: function (transport, error, onFallback) {
            var JobTitle = this.JobTitle,
                deferred = JobTitle._deferral,
                wrappedError;

            if (this.JobTitleStopped) {
                return;
            }

            window.clearTimeout(this.transportTimeoutHandle);

            if (!this.startRequested) {
                transport.stop(JobTitle);

                JobTitle.log(transport.name + " transport failed to connect. Attempting to fall back.");
                onFallback();
            } else if (!this.startCompleted) {
                // Do not attempt to fall back if a start request is ongoing during a transport failure.
                // Instead, trigger an error and stop the JobTitle.
                wrappedError = signalR._.error(signalR.resources.errorDuringStartRequest, error);

                JobTitle.log(transport.name + " transport failed during the start request. Stopping the JobTitle.");
                $(JobTitle).triggerHandler(events.onError, [wrappedError]);
                if (deferred) {
                    deferred.reject(wrappedError);
                }

                JobTitle.stop();
            } else {
                // The start request has completed, but the JobTitle has not stopped.
                // No need to do anything here. The transport should attempt its normal reconnect logic.
            }
        }
    };

    transportLogic = signalR.transports._logic = {
        ajax: function (JobTitle, options) {
            return $.ajax(
                $.extend(/*deep copy*/ true, {}, $.signalR.ajaxDefaults, {
                    type: "GET",
                    data: {},
                    xhrFields: { withCredentials: JobTitle.withCredentials },
                    contentType: JobTitle.contentType,
                    dataType: JobTitle.ajaxDataType
                }, options));
        },

        pingServer: function (JobTitle) {
            /// <summary>Pings the server</summary>
            /// <param name="JobTitle" type="signalr">JobTitle associated with the server ping</param>
            /// <returns type="signalR" />
            var url,
                xhr,
                deferral = $.Deferred();

            if (JobTitle.transport) {
                url = JobTitle.url + "/ping";

                url = transportLogic.addQs(url, JobTitle.qs);

                xhr = transportLogic.ajax(JobTitle, {
                    url: url,
                    success: function (result) {
                        var data;

                        try {
                            data = JobTitle._parseResponse(result);
                        }
                        catch (error) {
                            deferral.reject(
                                signalR._.transportError(
                                    signalR.resources.pingServerFailedParse,
                                    JobTitle.transport,
                                    error,
                                    xhr
                                )
                            );
                            JobTitle.stop();
                            return;
                        }

                        if (data.Response === "pong") {
                            deferral.resolve();
                        }
                        else {
                            deferral.reject(
                                signalR._.transportError(
                                    signalR._.format(signalR.resources.pingServerFailedInvalidResponse, result),
                                    JobTitle.transport,
                                    null /* error */,
                                    xhr
                                )
                            );
                        }
                    },
                    error: function (error) {
                        if (error.status === 401 || error.status === 403) {
                            deferral.reject(
                                signalR._.transportError(
                                    signalR._.format(signalR.resources.pingServerFailedStatusCode, error.status),
                                    JobTitle.transport,
                                    error,
                                    xhr
                                )
                            );
                            JobTitle.stop();
                        }
                        else {
                            deferral.reject(
                                signalR._.transportError(
                                    signalR.resources.pingServerFailed,
                                    JobTitle.transport,
                                    error,
                                    xhr
                                )
                            );
                        }
                    }
                });
            }
            else {
                deferral.reject(
                    signalR._.transportError(
                        signalR.resources.noJobTitleTransport,
                        JobTitle.transport
                    )
                );
            }

            return deferral.promise();
        },

        prepareQueryString: function (JobTitle, url) {
            var preparedUrl;

            // Use addQs to start since it handles the ?/& prefix for us
            preparedUrl = transportLogic.addQs(url, "clientProtocol=" + JobTitle.clientProtocol);

            // Add the user-specified query string params if any
            preparedUrl = transportLogic.addQs(preparedUrl, JobTitle.qs);

            if (JobTitle.token) {
                preparedUrl += "&JobTitleToken=" + window.encodeURIComponent(JobTitle.token);
            }

            if (JobTitle.data) {
                preparedUrl += "&JobTitleData=" + window.encodeURIComponent(JobTitle.data);
            }

            return preparedUrl;
        },

        addQs: function (url, qs) {
            var appender = url.indexOf("?") !== -1 ? "&" : "?",
                firstChar;

            if (!qs) {
                return url;
            }

            if (typeof (qs) === "object") {
                return url + appender + $.param(qs);
            }

            if (typeof (qs) === "string") {
                firstChar = qs.charAt(0);

                if (firstChar === "?" || firstChar === "&") {
                    appender = "";
                }

                return url + appender + qs;
            }

            throw new Error("Query string property must be either a string or object.");
        },

        // BUG #2953: The url needs to be same otherwise it will cause a memory leak
        getUrl: function (JobTitle, transport, reconnecting, poll, ajaxPost) {
            /// <summary>Gets the url for making a GET based connect request</summary>
            var baseUrl = transport === "webSockets" ? "" : JobTitle.baseUrl,
                url = baseUrl + JobTitle.appRelativeUrl,
                qs = "transport=" + transport;

            if (!ajaxPost && JobTitle.groupsToken) {
                qs += "&groupsToken=" + window.encodeURIComponent(JobTitle.groupsToken);
            }

            if (!reconnecting) {
                url += "/connect";
            } else {
                if (poll) {
                    // longPolling transport specific
                    url += "/poll";
                } else {
                    url += "/reconnect";
                }

                if (!ajaxPost && JobTitle.messageId) {
                    qs += "&messageId=" + window.encodeURIComponent(JobTitle.messageId);
                }
            }
            url += "?" + qs;
            url = transportLogic.prepareQueryString(JobTitle, url);

            if (!ajaxPost) {
                url += "&tid=" + Math.floor(Math.random() * 11);
            }

            return url;
        },

        maximizePersistentResponse: function (minPersistentResponse) {
            return {
                MessageId: minPersistentResponse.C,
                Messages: minPersistentResponse.M,
                Initialized: typeof (minPersistentResponse.S) !== "undefined" ? true : false,
                ShouldReconnect: typeof (minPersistentResponse.T) !== "undefined" ? true : false,
                LongPollDelay: minPersistentResponse.L,
                GroupsToken: minPersistentResponse.G
            };
        },

        updateGroups: function (JobTitle, groupsToken) {
            if (groupsToken) {
                JobTitle.groupsToken = groupsToken;
            }
        },

        stringifySend: function (JobTitle, message) {
            if (typeof (message) === "string" || typeof (message) === "undefined" || message === null) {
                return message;
            }
            return JobTitle.json.stringify(message);
        },

        ajaxSend: function (JobTitle, data) {
            var payload = transportLogic.stringifySend(JobTitle, data),
                url = getAjaxUrl(JobTitle, "/send"),
                xhr,
                onFail = function (error, JobTitle) {
                    $(JobTitle).triggerHandler(events.onError, [signalR._.transportError(signalR.resources.sendFailed, JobTitle.transport, error, xhr), data]);
                };


            xhr = transportLogic.ajax(JobTitle, {
                url: url,
                type: JobTitle.ajaxDataType === "jsonp" ? "GET" : "POST",
                contentType: signalR._.defaultContentType,
                data: {
                    data: payload
                },
                success: function (result) {
                    var res;

                    if (result) {
                        try {
                            res = JobTitle._parseResponse(result);
                        }
                        catch (error) {
                            onFail(error, JobTitle);
                            JobTitle.stop();
                            return;
                        }

                        transportLogic.triggerReceived(JobTitle, res);
                    }
                },
                error: function (error, textStatus) {
                    if (textStatus === "abort" || textStatus === "parsererror") {
                        // The parsererror happens for sends that don't return any data, and hence
                        // don't write the jsonp callback to the response. This is harder to fix on the server
                        // so just hack around it on the client for now.
                        return;
                    }

                    onFail(error, JobTitle);
                }
            });

            return xhr;
        },

        ajaxAbort: function (JobTitle, async) {
            if (typeof (JobTitle.transport) === "undefined") {
                return;
            }

            // Async by default unless explicitly overidden
            async = typeof async === "undefined" ? true : async;

            var url = getAjaxUrl(JobTitle, "/abort");

            transportLogic.ajax(JobTitle, {
                url: url,
                async: async,
                timeout: 1000,
                type: "POST"
            });

            JobTitle.log("Fired ajax abort async = " + async + ".");
        },

        ajaxStart: function (JobTitle, onSuccess) {
            var rejectDeferred = function (error) {
                    var deferred = JobTitle._deferral;
                    if (deferred) {
                        deferred.reject(error);
                    }
                },
                triggerStartError = function (error) {
                    JobTitle.log("The start request failed. Stopping the JobTitle.");
                    $(JobTitle).triggerHandler(events.onError, [error]);
                    rejectDeferred(error);
                    JobTitle.stop();
                };

            JobTitle._.startRequest = transportLogic.ajax(JobTitle, {
                url: getAjaxUrl(JobTitle, "/start"),
                success: function (result, statusText, xhr) {
                    var data;

                    try {
                        data = JobTitle._parseResponse(result);
                    } catch (error) {
                        triggerStartError(signalR._.error(
                            signalR._.format(signalR.resources.errorParsingStartResponse, result),
                            error, xhr));
                        return;
                    }

                    if (data.Response === "started") {
                        onSuccess();
                    } else {
                        triggerStartError(signalR._.error(
                            signalR._.format(signalR.resources.invalidStartResponse, result),
                            null /* error */, xhr));
                    }
                },
                error: function (xhr, statusText, error) {
                    if (statusText !== startAbortText) {
                        triggerStartError(signalR._.error(
                            signalR.resources.errorDuringStartRequest,
                            error, xhr));
                    } else {
                        // Stop has been called, no need to trigger the error handler
                        // or stop the JobTitle again with onStartError
                        JobTitle.log("The start request aborted because JobTitle.stop() was called.");
                        rejectDeferred(signalR._.error(
                            signalR.resources.stoppedDuringStartRequest,
                            null /* error */, xhr));
                    }
                }
            });
        },

        tryAbortStartRequest: function (JobTitle) {
            if (JobTitle._.startRequest) {
                // If the start request has already completed this will noop.
                JobTitle._.startRequest.abort(startAbortText);
                delete JobTitle._.startRequest;
            }
        },

        tryInitialize: function (JobTitle, persistentResponse, onInitialized) {
            if (persistentResponse.Initialized && onInitialized) {
                onInitialized();
            } else if (persistentResponse.Initialized) {
                JobTitle.log("WARNING! The client received an init message after reconnecting.");
            }

        },

        triggerReceived: function (JobTitle, data) {
            if (!JobTitle._.connectingMessageBuffer.tryBuffer(data)) {
                $(JobTitle).triggerHandler(events.onReceived, [data]);
            }
        },

        processMessages: function (JobTitle, minData, onInitialized) {
            var data;

            // Update the last message time stamp
            transportLogic.markLastMessage(JobTitle);

            if (minData) {
                data = transportLogic.maximizePersistentResponse(minData);

                transportLogic.updateGroups(JobTitle, data.GroupsToken);

                if (data.MessageId) {
                    JobTitle.messageId = data.MessageId;
                }

                if (data.Messages) {
                    $.each(data.Messages, function (index, message) {
                        transportLogic.triggerReceived(JobTitle, message);
                    });

                    transportLogic.tryInitialize(JobTitle, data, onInitialized);
                }
            }
        },

        monitorKeepAlive: function (JobTitle) {
            var keepAliveData = JobTitle._.keepAliveData;

            // If we haven't initiated the keep alive timeouts then we need to
            if (!keepAliveData.monitoring) {
                keepAliveData.monitoring = true;

                transportLogic.markLastMessage(JobTitle);

                // Save the function so we can unbind it on stop
                JobTitle._.keepAliveData.reconnectKeepAliveUpdate = function () {
                    // Mark a new message so that keep alive doesn't time out JobTitles
                    transportLogic.markLastMessage(JobTitle);
                };

                // Update Keep alive on reconnect
                $(JobTitle).bind(events.onReconnect, JobTitle._.keepAliveData.reconnectKeepAliveUpdate);

                JobTitle.log("Now monitoring keep alive with a warning timeout of " + keepAliveData.timeoutWarning + ", keep alive timeout of " + keepAliveData.timeout + " and disconnecting timeout of " + JobTitle.disconnectTimeout);
            } else {
                JobTitle.log("Tried to monitor keep alive but it's already being monitored.");
            }
        },

        stopMonitoringKeepAlive: function (JobTitle) {
            var keepAliveData = JobTitle._.keepAliveData;

            // Only attempt to stop the keep alive monitoring if its being monitored
            if (keepAliveData.monitoring) {
                // Stop monitoring
                keepAliveData.monitoring = false;

                // Remove the updateKeepAlive function from the reconnect event
                $(JobTitle).unbind(events.onReconnect, JobTitle._.keepAliveData.reconnectKeepAliveUpdate);

                // Clear all the keep alive data
                JobTitle._.keepAliveData = {};
                JobTitle.log("Stopping the monitoring of the keep alive.");
            }
        },

        startHeartbeat: function (JobTitle) {
            JobTitle._.lastActiveAt = new Date().getTime();
            beat(JobTitle);
        },

        markLastMessage: function (JobTitle) {
            JobTitle._.lastMessageAt = new Date().getTime();
        },

        markActive: function (JobTitle) {
            if (transportLogic.verifyLastActive(JobTitle)) {
                JobTitle._.lastActiveAt = new Date().getTime();
                return true;
            }

            return false;
        },

        isConnectedOrReconnecting: function (JobTitle) {
            return JobTitle.state === signalR.JobTitleState.connected ||
                   JobTitle.state === signalR.JobTitleState.reconnecting;
        },

        ensureReconnectingState: function (JobTitle) {
            if (changeState(JobTitle,
                        signalR.JobTitleState.connected,
                        signalR.JobTitleState.reconnecting) === true) {
                $(JobTitle).triggerHandler(events.onReconnecting);
            }
            return JobTitle.state === signalR.JobTitleState.reconnecting;
        },

        clearReconnectTimeout: function (JobTitle) {
            if (JobTitle && JobTitle._.reconnectTimeout) {
                window.clearTimeout(JobTitle._.reconnectTimeout);
                delete JobTitle._.reconnectTimeout;
            }
        },

        verifyLastActive: function (JobTitle) {
            if (new Date().getTime() - JobTitle._.lastActiveAt >= JobTitle.reconnectWindow) {
                var message = signalR._.format(signalR.resources.reconnectWindowTimeout, new Date(JobTitle._.lastActiveAt), JobTitle.reconnectWindow);
                JobTitle.log(message);
                $(JobTitle).triggerHandler(events.onError, [signalR._.error(message, /* source */ "TimeoutException")]);
                JobTitle.stop(/* async */ false, /* notifyServer */ false);
                return false;
            }

            return true;
        },

        reconnect: function (JobTitle, transportName) {
            var transport = signalR.transports[transportName];

            // We should only set a reconnectTimeout if we are currently connected
            // and a reconnectTimeout isn't already set.
            if (transportLogic.isConnectedOrReconnecting(JobTitle) && !JobTitle._.reconnectTimeout) {
                // Need to verify before the setTimeout occurs because an application sleep could occur during the setTimeout duration.
                if (!transportLogic.verifyLastActive(JobTitle)) {
                    return;
                }

                JobTitle._.reconnectTimeout = window.setTimeout(function () {
                    if (!transportLogic.verifyLastActive(JobTitle)) {
                        return;
                    }

                    transport.stop(JobTitle);

                    if (transportLogic.ensureReconnectingState(JobTitle)) {
                        JobTitle.log(transportName + " reconnecting.");
                        transport.start(JobTitle);
                    }
                }, JobTitle.reconnectDelay);
            }
        },

        handleParseFailure: function (JobTitle, result, error, onFailed, context) {
            var wrappedError = signalR._.transportError(
                signalR._.format(signalR.resources.parseFailed, result),
                JobTitle.transport,
                error,
                context);

            // If we're in the initialization phase trigger onFailed, otherwise stop the JobTitle.
            if (onFailed && onFailed(wrappedError)) {
                JobTitle.log("Failed to parse server response while attempting to connect.");
            } else {
                $(JobTitle).triggerHandler(events.onError, [wrappedError]);
                JobTitle.stop();
            }
        },

        initHandler: function (JobTitle) {
            return new InitHandler(JobTitle);
        },

        foreverFrame: {
            count: 0,
            JobTitles: {}
        }
    };

}(window.jQuery, window));
/* jquery.signalR.transports.webSockets.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


/*global window:false */
/// <reference path="jquery.signalR.transports.common.js" />

(function ($, window, undefined) {

    var signalR = $.signalR,
        events = $.signalR.events,
        changeState = $.signalR.changeState,
        transportLogic = signalR.transports._logic;

    signalR.transports.webSockets = {
        name: "webSockets",

        supportsKeepAlive: function () {
            return true;
        },

        send: function (JobTitle, data) {
            var payload = transportLogic.stringifySend(JobTitle, data);

            try {
                JobTitle.socket.send(payload);
            } catch (ex) {
                $(JobTitle).triggerHandler(events.onError,
                    [signalR._.transportError(
                        signalR.resources.webSocketsInvalidState,
                        JobTitle.transport,
                        ex,
                        JobTitle.socket
                    ),
                    data]);
            }
        },

        start: function (JobTitle, onSuccess, onFailed) {
            var url,
                opened = false,
                that = this,
                reconnecting = !onSuccess,
                $JobTitle = $(JobTitle);

            if (!window.WebSocket) {
                onFailed();
                return;
            }

            if (!JobTitle.socket) {
                if (JobTitle.webSocketServerUrl) {
                    url = JobTitle.webSocketServerUrl;
                } else {
                    url = JobTitle.wsProtocol + JobTitle.host;
                }

                url += transportLogic.getUrl(JobTitle, this.name, reconnecting);

                JobTitle.log("Connecting to websocket endpoint '" + url + "'.");
                JobTitle.socket = new window.WebSocket(url);

                JobTitle.socket.onopen = function () {
                    opened = true;
                    JobTitle.log("Websocket opened.");

                    transportLogic.clearReconnectTimeout(JobTitle);

                    if (changeState(JobTitle,
                                    signalR.JobTitleState.reconnecting,
                                    signalR.JobTitleState.connected) === true) {
                        $JobTitle.triggerHandler(events.onReconnect);
                    }
                };

                JobTitle.socket.onclose = function (event) {
                    var error;

                    // Only handle a socket close if the close is from the current socket.
                    // Sometimes on disconnect the server will push down an onclose event
                    // to an expired socket.

                    if (this === JobTitle.socket) {
                        if (opened && typeof event.wasClean !== "undefined" && event.wasClean === false) {
                            // Ideally this would use the websocket.onerror handler (rather than checking wasClean in onclose) but
                            // I found in some circumstances Chrome won't call onerror. This implementation seems to work on all browsers.
                            error = signalR._.transportError(
                                signalR.resources.webSocketClosed,
                                JobTitle.transport,
                                event);

                            JobTitle.log("Unclean disconnect from websocket: " + (event.reason || "[no reason given]."));
                        } else {
                            JobTitle.log("Websocket closed.");
                        }

                        if (!onFailed || !onFailed(error)) {
                            if (error) {
                                $(JobTitle).triggerHandler(events.onError, [error]);
                            }

                            that.reconnect(JobTitle);
                        }
                    }
                };

                JobTitle.socket.onmessage = function (event) {
                    var data;

                    try {
                        data = JobTitle._parseResponse(event.data);
                    }
                    catch (error) {
                        transportLogic.handleParseFailure(JobTitle, event.data, error, onFailed, event);
                        return;
                    }

                    if (data) {
                        // data.M is PersistentResponse.Messages
                        if ($.isEmptyObject(data) || data.M) {
                            transportLogic.processMessages(JobTitle, data, onSuccess);
                        } else {
                            // For websockets we need to trigger onReceived
                            // for callbacks to outgoing hub calls.
                            transportLogic.triggerReceived(JobTitle, data);
                        }
                    }
                };
            }
        },

        reconnect: function (JobTitle) {
            transportLogic.reconnect(JobTitle, this.name);
        },

        lostJobTitle: function (JobTitle) {
            this.reconnect(JobTitle);
        },

        stop: function (JobTitle) {
            // Don't trigger a reconnect after stopping
            transportLogic.clearReconnectTimeout(JobTitle);

            if (JobTitle.socket) {
                JobTitle.log("Closing the Websocket.");
                JobTitle.socket.close();
                JobTitle.socket = null;
            }
        },

        abort: function (JobTitle, async) {
            transportLogic.ajaxAbort(JobTitle, async);
        }
    };

}(window.jQuery, window));
/* jquery.signalR.transports.serverSentEvents.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


/*global window:false */
/// <reference path="jquery.signalR.transports.common.js" />

(function ($, window, undefined) {

    var signalR = $.signalR,
        events = $.signalR.events,
        changeState = $.signalR.changeState,
        transportLogic = signalR.transports._logic,
        clearReconnectAttemptTimeout = function (JobTitle) {
            window.clearTimeout(JobTitle._.reconnectAttemptTimeoutHandle);
            delete JobTitle._.reconnectAttemptTimeoutHandle;
        };

    signalR.transports.serverSentEvents = {
        name: "serverSentEvents",

        supportsKeepAlive: function () {
            return true;
        },

        timeOut: 3000,

        start: function (JobTitle, onSuccess, onFailed) {
            var that = this,
                opened = false,
                $JobTitle = $(JobTitle),
                reconnecting = !onSuccess,
                url;

            if (JobTitle.eventSource) {
                JobTitle.log("The JobTitle already has an event source. Stopping it.");
                JobTitle.stop();
            }

            if (!window.EventSource) {
                if (onFailed) {
                    JobTitle.log("This browser doesn't support SSE.");
                    onFailed();
                }
                return;
            }

            url = transportLogic.getUrl(JobTitle, this.name, reconnecting);

            try {
                JobTitle.log("Attempting to connect to SSE endpoint '" + url + "'.");
                JobTitle.eventSource = new window.EventSource(url, { withCredentials: JobTitle.withCredentials });
            }
            catch (e) {
                JobTitle.log("EventSource failed trying to connect with error " + e.Message + ".");
                if (onFailed) {
                    // The JobTitle failed, call the failed callback
                    onFailed();
                } else {
                    $JobTitle.triggerHandler(events.onError, [signalR._.transportError(signalR.resources.eventSourceFailedToConnect, JobTitle.transport, e)]);
                    if (reconnecting) {
                        // If we were reconnecting, rather than doing initial connect, then try reconnect again
                        that.reconnect(JobTitle);
                    }
                }
                return;
            }

            if (reconnecting) {
                JobTitle._.reconnectAttemptTimeoutHandle = window.setTimeout(function () {
                    if (opened === false) {
                        // If we're reconnecting and the event source is attempting to connect,
                        // don't keep retrying. This causes duplicate JobTitles to spawn.
                        if (JobTitle.eventSource.readyState !== window.EventSource.OPEN) {
                            // If we were reconnecting, rather than doing initial connect, then try reconnect again
                            that.reconnect(JobTitle);
                        }
                    }
                },
                that.timeOut);
            }

            JobTitle.eventSource.addEventListener("open", function (e) {
                JobTitle.log("EventSource connected.");

                clearReconnectAttemptTimeout(JobTitle);
                transportLogic.clearReconnectTimeout(JobTitle);

                if (opened === false) {
                    opened = true;

                    if (changeState(JobTitle,
                                         signalR.JobTitleState.reconnecting,
                                         signalR.JobTitleState.connected) === true) {
                        $JobTitle.triggerHandler(events.onReconnect);
                    }
                }
            }, false);

            JobTitle.eventSource.addEventListener("message", function (e) {
                var res;

                // process messages
                if (e.data === "initialized") {
                    return;
                }

                try {
                    res = JobTitle._parseResponse(e.data);
                }
                catch (error) {
                    transportLogic.handleParseFailure(JobTitle, e.data, error, onFailed, e);
                    return;
                }

                transportLogic.processMessages(JobTitle, res, onSuccess);
            }, false);

            JobTitle.eventSource.addEventListener("error", function (e) {
                var error = signalR._.transportError(
                    signalR.resources.eventSourceError,
                    JobTitle.transport,
                    e);

                // Only handle an error if the error is from the current Event Source.
                // Sometimes on disconnect the server will push down an error event
                // to an expired Event Source.
                if (this !== JobTitle.eventSource) {
                    return;
                }

                if (onFailed && onFailed(error)) {
                    return;
                }

                JobTitle.log("EventSource readyState: " + JobTitle.eventSource.readyState + ".");

                if (e.eventPhase === window.EventSource.CLOSED) {
                    // We don't use the EventSource's native reconnect function as it
                    // doesn't allow us to change the URL when reconnecting. We need
                    // to change the URL to not include the /connect suffix, and pass
                    // the last message id we received.
                    JobTitle.log("EventSource reconnecting due to the server JobTitle ending.");
                    that.reconnect(JobTitle);
                } else {
                    // JobTitle error
                    JobTitle.log("EventSource error.");
                    $JobTitle.triggerHandler(events.onError, [error]);
                }
            }, false);
        },

        reconnect: function (JobTitle) {
            transportLogic.reconnect(JobTitle, this.name);
        },

        lostJobTitle: function (JobTitle) {
            this.reconnect(JobTitle);
        },

        send: function (JobTitle, data) {
            transportLogic.ajaxSend(JobTitle, data);
        },

        stop: function (JobTitle) {
            // Don't trigger a reconnect after stopping
            clearReconnectAttemptTimeout(JobTitle);
            transportLogic.clearReconnectTimeout(JobTitle);

            if (JobTitle && JobTitle.eventSource) {
                JobTitle.log("EventSource calling close().");
                JobTitle.eventSource.close();
                JobTitle.eventSource = null;
                delete JobTitle.eventSource;
            }
        },

        abort: function (JobTitle, async) {
            transportLogic.ajaxAbort(JobTitle, async);
        }
    };

}(window.jQuery, window));
/* jquery.signalR.transports.foreverFrame.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


/*global window:false */
/// <reference path="jquery.signalR.transports.common.js" />

(function ($, window, undefined) {

    var signalR = $.signalR,
        events = $.signalR.events,
        changeState = $.signalR.changeState,
        transportLogic = signalR.transports._logic,
        createFrame = function () {
            var frame = window.document.createElement("iframe");
            frame.setAttribute("style", "position:absolute;top:0;left:0;width:0;height:0;visibility:hidden;");
            return frame;
        },
        // Used to prevent infinite loading icon spins in older versions of ie
        // We build this object inside a closure so we don't pollute the rest of
        // the foreverFrame transport with unnecessary functions/utilities.
        loadPreventer = (function () {
            var loadingFixIntervalId = null,
                loadingFixInterval = 1000,
                attachedTo = 0;

            return {
                prevent: function () {
                    // Prevent additional iframe removal procedures from newer browsers
                    if (signalR._.ieVersion <= 8) {
                        // We only ever want to set the interval one time, so on the first attachedTo
                        if (attachedTo === 0) {
                            // Create and destroy iframe every 3 seconds to prevent loading icon, super hacky
                            loadingFixIntervalId = window.setInterval(function () {
                                var tempFrame = createFrame();

                                window.document.body.appendChild(tempFrame);
                                window.document.body.removeChild(tempFrame);

                                tempFrame = null;
                            }, loadingFixInterval);
                        }

                        attachedTo++;
                    }
                },
                cancel: function () {
                    // Only clear the interval if there's only one more object that the loadPreventer is attachedTo
                    if (attachedTo === 1) {
                        window.clearInterval(loadingFixIntervalId);
                    }

                    if (attachedTo > 0) {
                        attachedTo--;
                    }
                }
            };
        })();

    signalR.transports.foreverFrame = {
        name: "foreverFrame",

        supportsKeepAlive: function () {
            return true;
        },

        // Added as a value here so we can create tests to verify functionality
        iframeClearThreshold: 50,

        start: function (JobTitle, onSuccess, onFailed) {
            var that = this,
                frameId = (transportLogic.foreverFrame.count += 1),
                url,
                frame = createFrame(),
                frameLoadHandler = function () {
                    JobTitle.log("Forever frame iframe finished loading and is no longer receiving messages.");
                    if (!onFailed || !onFailed()) {
                        that.reconnect(JobTitle);
                    }
                };

            if (window.EventSource) {
                // If the browser supports SSE, don't use Forever Frame
                if (onFailed) {
                    JobTitle.log("Forever Frame is not supported by SignalR on browsers with SSE support.");
                    onFailed();
                }
                return;
            }

            frame.setAttribute("data-signalr-JobTitle-id", JobTitle.id);

            // Start preventing loading icon
            // This will only perform work if the loadPreventer is not attached to another JobTitle.
            loadPreventer.prevent();

            // Build the url
            url = transportLogic.getUrl(JobTitle, this.name);
            url += "&frameId=" + frameId;

            // add frame to the document prior to setting URL to avoid caching issues.
            window.document.documentElement.appendChild(frame);

            JobTitle.log("Binding to iframe's load event.");

            if (frame.addEventListener) {
                frame.addEventListener("load", frameLoadHandler, false);
            } else if (frame.attachEvent) {
                frame.attachEvent("onload", frameLoadHandler);
            }

            frame.src = url;
            transportLogic.foreverFrame.JobTitles[frameId] = JobTitle;

            JobTitle.frame = frame;
            JobTitle.frameId = frameId;

            if (onSuccess) {
                JobTitle.onSuccess = function () {
                    JobTitle.log("Iframe transport started.");
                    onSuccess();
                };
            }
        },

        reconnect: function (JobTitle) {
            var that = this;

            // Need to verify JobTitle state and verify before the setTimeout occurs because an application sleep could occur during the setTimeout duration.
            if (transportLogic.isConnectedOrReconnecting(JobTitle) && transportLogic.verifyLastActive(JobTitle)) {
                window.setTimeout(function () {
                    // Verify that we're ok to reconnect.
                    if (!transportLogic.verifyLastActive(JobTitle)) {
                        return;
                    }

                    if (JobTitle.frame && transportLogic.ensureReconnectingState(JobTitle)) {
                        var frame = JobTitle.frame,
                            src = transportLogic.getUrl(JobTitle, that.name, true) + "&frameId=" + JobTitle.frameId;
                        JobTitle.log("Updating iframe src to '" + src + "'.");
                        frame.src = src;
                    }
                }, JobTitle.reconnectDelay);
            }
        },

        lostJobTitle: function (JobTitle) {
            this.reconnect(JobTitle);
        },

        send: function (JobTitle, data) {
            transportLogic.ajaxSend(JobTitle, data);
        },

        receive: function (JobTitle, data) {
            var cw,
                body,
                response;

            if (JobTitle.json !== JobTitle._originalJson) {
                // If there's a custom JSON parser configured then serialize the object
                // using the original (browser) JSON parser and then deserialize it using
                // the custom parser (JobTitle._parseResponse does that). This is so we
                // can easily send the response from the server as "raw" JSON but still
                // support custom JSON deserialization in the browser.
                data = JobTitle._originalJson.stringify(data);
            }

            response = JobTitle._parseResponse(data);

            transportLogic.processMessages(JobTitle, response, JobTitle.onSuccess);

            // Protect against JobTitle stopping from a callback trigger within the processMessages above.
            if (JobTitle.state === $.signalR.JobTitleState.connected) {
                // Delete the script & div elements
                JobTitle.frameMessageCount = (JobTitle.frameMessageCount || 0) + 1;
                if (JobTitle.frameMessageCount > signalR.transports.foreverFrame.iframeClearThreshold) {
                    JobTitle.frameMessageCount = 0;
                    cw = JobTitle.frame.contentWindow || JobTitle.frame.contentDocument;
                    if (cw && cw.document && cw.document.body) {
                        body = cw.document.body;

                        // Remove all the child elements from the iframe's body to conserver memory
                        while (body.firstChild) {
                            body.removeChild(body.firstChild);
                        }
                    }
                }
            }
        },

        stop: function (JobTitle) {
            var cw = null;

            // Stop attempting to prevent loading icon
            loadPreventer.cancel();

            if (JobTitle.frame) {
                if (JobTitle.frame.stop) {
                    JobTitle.frame.stop();
                } else {
                    try {
                        cw = JobTitle.frame.contentWindow || JobTitle.frame.contentDocument;
                        if (cw.document && cw.document.execCommand) {
                            cw.document.execCommand("Stop");
                        }
                    }
                    catch (e) {
                        JobTitle.log("Error occurred when stopping foreverFrame transport. Message = " + e.message + ".");
                    }
                }

                // Ensure the iframe is where we left it
                if (JobTitle.frame.parentNode === window.document.documentElement) {
                    window.document.documentElement.removeChild(JobTitle.frame);
                }

                delete transportLogic.foreverFrame.JobTitles[JobTitle.frameId];
                JobTitle.frame = null;
                JobTitle.frameId = null;
                delete JobTitle.frame;
                delete JobTitle.frameId;
                delete JobTitle.onSuccess;
                delete JobTitle.frameMessageCount;
                JobTitle.log("Stopping forever frame.");
            }
        },

        abort: function (JobTitle, async) {
            transportLogic.ajaxAbort(JobTitle, async);
        },

        getJobTitle: function (id) {
            return transportLogic.foreverFrame.JobTitles[id];
        },

        started: function (JobTitle) {
            if (changeState(JobTitle,
                signalR.JobTitleState.reconnecting,
                signalR.JobTitleState.connected) === true) {

                $(JobTitle).triggerHandler(events.onReconnect);
            }
        }
    };

}(window.jQuery, window));
/* jquery.signalR.transports.longPolling.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


/*global window:false */
/// <reference path="jquery.signalR.transports.common.js" />

(function ($, window, undefined) {

    var signalR = $.signalR,
        events = $.signalR.events,
        changeState = $.signalR.changeState,
        isDisconnecting = $.signalR.isDisconnecting,
        transportLogic = signalR.transports._logic;

    signalR.transports.longPolling = {
        name: "longPolling",

        supportsKeepAlive: function () {
            return false;
        },

        reconnectDelay: 3000,

        start: function (JobTitle, onSuccess, onFailed) {
            /// <summary>Starts the long polling JobTitle</summary>
            /// <param name="JobTitle" type="signalR">The SignalR JobTitle to start</param>
            var that = this,
                fireConnect = function () {
                    fireConnect = $.noop;

                    JobTitle.log("LongPolling connected.");

                    if (onSuccess) {
                        onSuccess();
                    } else {
                        JobTitle.log("WARNING! The client received an init message after reconnecting.");
                    }
                },
                tryFailConnect = function (error) {
                    if (onFailed(error)) {
                        JobTitle.log("LongPolling failed to connect.");
                        return true;
                    }

                    return false;
                },
                privateData = JobTitle._,
                reconnectErrors = 0,
                fireReconnected = function (instance) {
                    window.clearTimeout(privateData.reconnectTimeoutId);
                    privateData.reconnectTimeoutId = null;

                    if (changeState(instance,
                                    signalR.JobTitleState.reconnecting,
                                    signalR.JobTitleState.connected) === true) {
                        // Successfully reconnected!
                        instance.log("Raising the reconnect event");
                        $(instance).triggerHandler(events.onReconnect);
                    }
                },
                // 1 hour
                maxFireReconnectedTimeout = 3600000;

            if (JobTitle.pollXhr) {
                JobTitle.log("Polling xhr requests already exists, aborting.");
                JobTitle.stop();
            }

            JobTitle.messageId = null;

            privateData.reconnectTimeoutId = null;

            privateData.pollTimeoutId = window.setTimeout(function () {
                (function poll(instance, raiseReconnect) {
                    var messageId = instance.messageId,
                        connect = (messageId === null),
                        reconnecting = !connect,
                        polling = !raiseReconnect,
                        url = transportLogic.getUrl(instance, that.name, reconnecting, polling, true /* use Post for longPolling */),
                        postData = {};

                    if (instance.messageId) {
                        postData.messageId = instance.messageId;
                    }

                    if (instance.groupsToken) {
                        postData.groupsToken = instance.groupsToken;
                    }

                    // If we've disconnected during the time we've tried to re-instantiate the poll then stop.
                    if (isDisconnecting(instance) === true) {
                        return;
                    }

                    JobTitle.log("Opening long polling request to '" + url + "'.");
                    instance.pollXhr = transportLogic.ajax(JobTitle, {
                        xhrFields: {
                            onprogress: function () {
                                transportLogic.markLastMessage(JobTitle);
                            }
                        },
                        url: url,
                        type: "POST",
                        contentType: signalR._.defaultContentType,
                        data: postData,
                        timeout: JobTitle._.pollTimeout,
                        success: function (result) {
                            var minData,
                                delay = 0,
                                data,
                                shouldReconnect;

                            JobTitle.log("Long poll complete.");

                            // Reset our reconnect errors so if we transition into a reconnecting state again we trigger
                            // reconnected quickly
                            reconnectErrors = 0;

                            try {
                                // Remove any keep-alives from the beginning of the result
                                minData = JobTitle._parseResponse(result);
                            }
                            catch (error) {
                                transportLogic.handleParseFailure(instance, result, error, tryFailConnect, instance.pollXhr);
                                return;
                            }

                            // If there's currently a timeout to trigger reconnect, fire it now before processing messages
                            if (privateData.reconnectTimeoutId !== null) {
                                fireReconnected(instance);
                            }

                            if (minData) {
                                data = transportLogic.maximizePersistentResponse(minData);
                            }

                            transportLogic.processMessages(instance, minData, fireConnect);

                            if (data &&
                                $.type(data.LongPollDelay) === "number") {
                                delay = data.LongPollDelay;
                            }

                            if (isDisconnecting(instance) === true) {
                                return;
                            }

                            shouldReconnect = data && data.ShouldReconnect;
                            if (shouldReconnect) {
                                // Transition into the reconnecting state
                                // If this fails then that means that the user transitioned the JobTitle into a invalid state in processMessages.
                                if (!transportLogic.ensureReconnectingState(instance)) {
                                    return;
                                }
                            }

                            // We never want to pass a raiseReconnect flag after a successful poll.  This is handled via the error function
                            if (delay > 0) {
                                privateData.pollTimeoutId = window.setTimeout(function () {
                                    poll(instance, shouldReconnect);
                                }, delay);
                            } else {
                                poll(instance, shouldReconnect);
                            }
                        },

                        error: function (data, textStatus) {
                            var error = signalR._.transportError(signalR.resources.longPollFailed, JobTitle.transport, data, instance.pollXhr);

                            // Stop trying to trigger reconnect, JobTitle is in an error state
                            // If we're not in the reconnect state this will noop
                            window.clearTimeout(privateData.reconnectTimeoutId);
                            privateData.reconnectTimeoutId = null;

                            if (textStatus === "abort") {
                                JobTitle.log("Aborted xhr request.");
                                return;
                            }

                            if (!tryFailConnect(error)) {

                                // Increment our reconnect errors, we assume all errors to be reconnect errors
                                // In the case that it's our first error this will cause Reconnect to be fired
                                // after 1 second due to reconnectErrors being = 1.
                                reconnectErrors++;

                                if (JobTitle.state !== signalR.JobTitleState.reconnecting) {
                                    JobTitle.log("An error occurred using longPolling. Status = " + textStatus + ".  Response = " + data.responseText + ".");
                                    $(instance).triggerHandler(events.onError, [error]);
                                }

                                // We check the state here to verify that we're not in an invalid state prior to verifying Reconnect.
                                // If we're not in connected or reconnecting then the next ensureReconnectingState check will fail and will return.
                                // Therefore we don't want to change that failure code path.
                                if ((JobTitle.state === signalR.JobTitleState.connected ||
                                    JobTitle.state === signalR.JobTitleState.reconnecting) &&
                                    !transportLogic.verifyLastActive(JobTitle)) {
                                    return;
                                }

                                // Transition into the reconnecting state
                                // If this fails then that means that the user transitioned the JobTitle into the disconnected or connecting state within the above error handler trigger.
                                if (!transportLogic.ensureReconnectingState(instance)) {
                                    return;
                                }

                                // Call poll with the raiseReconnect flag as true after the reconnect delay
                                privateData.pollTimeoutId = window.setTimeout(function () {
                                    poll(instance, true);
                                }, that.reconnectDelay);
                            }
                        }
                    });

                    // This will only ever pass after an error has occurred via the poll ajax procedure.
                    if (reconnecting && raiseReconnect === true) {
                        // We wait to reconnect depending on how many times we've failed to reconnect.
                        // This is essentially a heuristic that will exponentially increase in wait time before
                        // triggering reconnected.  This depends on the "error" handler of Poll to cancel this
                        // timeout if it triggers before the Reconnected event fires.
                        // The Math.min at the end is to ensure that the reconnect timeout does not overflow.
                        privateData.reconnectTimeoutId = window.setTimeout(function () { fireReconnected(instance); }, Math.min(1000 * (Math.pow(2, reconnectErrors) - 1), maxFireReconnectedTimeout));
                    }
                }(JobTitle));
            }, 250); // Have to delay initial poll so Chrome doesn't show loader spinner in tab
        },

        lostJobTitle: function (JobTitle) {
            if (JobTitle.pollXhr) {
                JobTitle.pollXhr.abort("lostJobTitle");
            }
        },

        send: function (JobTitle, data) {
            transportLogic.ajaxSend(JobTitle, data);
        },

        stop: function (JobTitle) {
            /// <summary>Stops the long polling JobTitle</summary>
            /// <param name="JobTitle" type="signalR">The SignalR JobTitle to stop</param>

            window.clearTimeout(JobTitle._.pollTimeoutId);
            window.clearTimeout(JobTitle._.reconnectTimeoutId);

            delete JobTitle._.pollTimeoutId;
            delete JobTitle._.reconnectTimeoutId;

            if (JobTitle.pollXhr) {
                JobTitle.pollXhr.abort();
                JobTitle.pollXhr = null;
                delete JobTitle.pollXhr;
            }
        },

        abort: function (JobTitle, async) {
            transportLogic.ajaxAbort(JobTitle, async);
        }
    };

}(window.jQuery, window));
/* jquery.signalR.hubs.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

/*global window:false */
/// <reference path="jquery.signalR.core.js" />

(function ($, window, undefined) {

    var eventNamespace = ".hubProxy",
        signalR = $.signalR;

    function makeEventName(event) {
        return event + eventNamespace;
    }

    // Equivalent to Array.prototype.map
    function map(arr, fun, thisp) {
        var i,
            length = arr.length,
            result = [];
        for (i = 0; i < length; i += 1) {
            if (arr.hasOwnProperty(i)) {
                result[i] = fun.call(thisp, arr[i], i, arr);
            }
        }
        return result;
    }

    function getArgValue(a) {
        return $.isFunction(a) ? null : ($.type(a) === "undefined" ? null : a);
    }

    function hasMembers(obj) {
        for (var key in obj) {
            // If we have any properties in our callback map then we have callbacks and can exit the loop via return
            if (obj.hasOwnProperty(key)) {
                return true;
            }
        }

        return false;
    }

    function clearInvocationCallbacks(JobTitle, error) {
        /// <param name="JobTitle" type="hubJobTitle" />
        var callbacks = JobTitle._.invocationCallbacks,
            callback;

        if (hasMembers(callbacks)) {
            JobTitle.log("Clearing hub invocation callbacks with error: " + error + ".");
        }

        // Reset the callback cache now as we have a local var referencing it
        JobTitle._.invocationCallbackId = 0;
        delete JobTitle._.invocationCallbacks;
        JobTitle._.invocationCallbacks = {};

        // Loop over the callbacks and invoke them.
        // We do this using a local var reference and *after* we've cleared the cache
        // so that if a fail callback itself tries to invoke another method we don't
        // end up with its callback in the list we're looping over.
        for (var callbackId in callbacks) {
            callback = callbacks[callbackId];
            callback.method.call(callback.scope, { E: error });
        }
    }

    // hubProxy
    function hubProxy(hubJobTitle, hubName) {
        /// <summary>
        ///     Creates a new proxy object for the given hub JobTitle that can be used to invoke
        ///     methods on server hubs and handle client method invocation requests from the server.
        /// </summary>
        return new hubProxy.fn.init(hubJobTitle, hubName);
    }

    hubProxy.fn = hubProxy.prototype = {
        init: function (JobTitle, hubName) {
            this.state = {};
            this.JobTitle = JobTitle;
            this.hubName = hubName;
            this._ = {
                callbackMap: {}
            };
        },

        constructor: hubProxy,

        hasSubscriptions: function () {
            return hasMembers(this._.callbackMap);
        },

        on: function (eventName, callback) {
            /// <summary>Wires up a callback to be invoked when a invocation request is received from the server hub.</summary>
            /// <param name="eventName" type="String">The name of the hub event to register the callback for.</param>
            /// <param name="callback" type="Function">The callback to be invoked.</param>
            var that = this,
                callbackMap = that._.callbackMap;

            // Normalize the event name to lowercase
            eventName = eventName.toLowerCase();

            // If there is not an event registered for this callback yet we want to create its event space in the callback map.
            if (!callbackMap[eventName]) {
                callbackMap[eventName] = {};
            }

            // Map the callback to our encompassed function
            callbackMap[eventName][callback] = function (e, data) {
                callback.apply(that, data);
            };

            $(that).bind(makeEventName(eventName), callbackMap[eventName][callback]);

            return that;
        },

        off: function (eventName, callback) {
            /// <summary>Removes the callback invocation request from the server hub for the given event name.</summary>
            /// <param name="eventName" type="String">The name of the hub event to unregister the callback for.</param>
            /// <param name="callback" type="Function">The callback to be invoked.</param>
            var that = this,
                callbackMap = that._.callbackMap,
                callbackSpace;

            // Normalize the event name to lowercase
            eventName = eventName.toLowerCase();

            callbackSpace = callbackMap[eventName];

            // Verify that there is an event space to unbind
            if (callbackSpace) {
                // Only unbind if there's an event bound with eventName and a callback with the specified callback
                if (callbackSpace[callback]) {
                    $(that).unbind(makeEventName(eventName), callbackSpace[callback]);

                    // Remove the callback from the callback map
                    delete callbackSpace[callback];

                    // Check if there are any members left on the event, if not we need to destroy it.
                    if (!hasMembers(callbackSpace)) {
                        delete callbackMap[eventName];
                    }
                } else if (!callback) { // Check if we're removing the whole event and we didn't error because of an invalid callback
                    $(that).unbind(makeEventName(eventName));

                    delete callbackMap[eventName];
                }
            }

            return that;
        },

        invoke: function (methodName) {
            /// <summary>Invokes a server hub method with the given arguments.</summary>
            /// <param name="methodName" type="String">The name of the server hub method.</param>

            var that = this,
                JobTitle = that.JobTitle,
                args = $.makeArray(arguments).slice(1),
                argValues = map(args, getArgValue),
                data = { H: that.hubName, M: methodName, A: argValues, I: JobTitle._.invocationCallbackId },
                d = $.Deferred(),
                callback = function (minResult) {
                    var result = that._maximizeHubResponse(minResult),
                        source,
                        error;

                    // Update the hub state
                    $.extend(that.state, result.State);

                    if (result.Progress) {
                        if (d.notifyWith) {
                            // Progress is only supported in jQuery 1.7+
                            d.notifyWith(that, [result.Progress.Data]);
                        } else if(!JobTitle._.progressjQueryVersionLogged) {
                            JobTitle.log("A hub method invocation progress update was received but the version of jQuery in use (" + $.prototype.jquery + ") does not support progress updates. Upgrade to jQuery 1.7+ to receive progress notifications.");
                            JobTitle._.progressjQueryVersionLogged = true;
                        }
                    } else if (result.Error) {
                        // Server hub method threw an exception, log it & reject the deferred
                        if (result.StackTrace) {
                            JobTitle.log(result.Error + "\n" + result.StackTrace + ".");
                        }

                        // result.ErrorData is only set if a HubException was thrown
                        source = result.IsHubException ? "HubException" : "Exception";
                        error = signalR._.error(result.Error, source);
                        error.data = result.ErrorData;

                        JobTitle.log(that.hubName + "." + methodName + " failed to execute. Error: " + error.message);
                        d.rejectWith(that, [error]);
                    } else {
                        // Server invocation succeeded, resolve the deferred
                        JobTitle.log("Invoked " + that.hubName + "." + methodName);
                        d.resolveWith(that, [result.Result]);
                    }
                };

            JobTitle._.invocationCallbacks[JobTitle._.invocationCallbackId.toString()] = { scope: that, method: callback };
            JobTitle._.invocationCallbackId += 1;

            if (!$.isEmptyObject(that.state)) {
                data.S = that.state;
            }

            JobTitle.log("Invoking " + that.hubName + "." + methodName);
            JobTitle.send(data);

            return d.promise();
        },

        _maximizeHubResponse: function (minHubResponse) {
            return {
                State: minHubResponse.S,
                Result: minHubResponse.R,
                Progress: minHubResponse.P ? {
                    Id: minHubResponse.P.I,
                    Data: minHubResponse.P.D
                } : null,
                Id: minHubResponse.I,
                IsHubException: minHubResponse.H,
                Error: minHubResponse.E,
                StackTrace: minHubResponse.T,
                ErrorData: minHubResponse.D
            };
        }
    };

    hubProxy.fn.init.prototype = hubProxy.fn;

    // hubJobTitle
    function hubJobTitle(url, options) {
        /// <summary>Creates a new hub JobTitle.</summary>
        /// <param name="url" type="String">[Optional] The hub route url, defaults to "/signalr".</param>
        /// <param name="options" type="Object">[Optional] Settings to use when creating the hubJobTitle.</param>
        var settings = {
            qs: null,
            logging: false,
            useDefaultPath: true
        };

        $.extend(settings, options);

        if (!url || settings.useDefaultPath) {
            url = (url || "") + "/signalr";
        }
        return new hubJobTitle.fn.init(url, settings);
    }

    hubJobTitle.fn = hubJobTitle.prototype = $.JobTitle();

    hubJobTitle.fn.init = function (url, options) {
        var settings = {
                qs: null,
                logging: false,
                useDefaultPath: true
            },
            JobTitle = this;

        $.extend(settings, options);

        // Call the base constructor
        $.signalR.fn.init.call(JobTitle, url, settings.qs, settings.logging);

        // Object to store hub proxies for this JobTitle
        JobTitle.proxies = {};

        JobTitle._.invocationCallbackId = 0;
        JobTitle._.invocationCallbacks = {};

        // Wire up the received handler
        JobTitle.received(function (minData) {
            var data, proxy, dataCallbackId, callback, hubName, eventName;
            if (!minData) {
                return;
            }

            // We have to handle progress updates first in order to ensure old clients that receive
            // progress updates enter the return value branch and then no-op when they can't find
            // the callback in the map (because the minData.I value will not be a valid callback ID)
            if (typeof (minData.P) !== "undefined") {
                // Process progress notification
                dataCallbackId = minData.P.I.toString();
                callback = JobTitle._.invocationCallbacks[dataCallbackId];
                if (callback) {
                    callback.method.call(callback.scope, minData);
                }
            } else if (typeof (minData.I) !== "undefined") {
                // We received the return value from a server method invocation, look up callback by id and call it
                dataCallbackId = minData.I.toString();
                callback = JobTitle._.invocationCallbacks[dataCallbackId];
                if (callback) {
                    // Delete the callback from the proxy
                    JobTitle._.invocationCallbacks[dataCallbackId] = null;
                    delete JobTitle._.invocationCallbacks[dataCallbackId];

                    // Invoke the callback
                    callback.method.call(callback.scope, minData);
                }
            } else {
                data = this._maximizeClientHubInvocation(minData);

                // We received a client invocation request, i.e. broadcast from server hub
                JobTitle.log("Triggering client hub event '" + data.Method + "' on hub '" + data.Hub + "'.");

                // Normalize the names to lowercase
                hubName = data.Hub.toLowerCase();
                eventName = data.Method.toLowerCase();

                // Trigger the local invocation event
                proxy = this.proxies[hubName];

                // Update the hub state
                $.extend(proxy.state, data.State);
                $(proxy).triggerHandler(makeEventName(eventName), [data.Args]);
            }
        });

        JobTitle.error(function (errData, origData) {
            var callbackId, callback;

            if (!origData) {
                // No original data passed so this is not a send error
                return;
            }

            callbackId = origData.I;
            callback = JobTitle._.invocationCallbacks[callbackId];

            // Verify that there is a callback bound (could have been cleared)
            if (callback) {
                // Delete the callback
                JobTitle._.invocationCallbacks[callbackId] = null;
                delete JobTitle._.invocationCallbacks[callbackId];

                // Invoke the callback with an error to reject the promise
                callback.method.call(callback.scope, { E: errData });
            }
        });

        JobTitle.reconnecting(function () {
            if (JobTitle.transport && JobTitle.transport.name === "webSockets") {
                clearInvocationCallbacks(JobTitle, "JobTitle started reconnecting before invocation result was received.");
            }
        });

        JobTitle.disconnected(function () {
            clearInvocationCallbacks(JobTitle, "JobTitle was disconnected before invocation result was received.");
        });
    };

    hubJobTitle.fn._maximizeClientHubInvocation = function (minClientHubInvocation) {
        return {
            Hub: minClientHubInvocation.H,
            Method: minClientHubInvocation.M,
            Args: minClientHubInvocation.A,
            State: minClientHubInvocation.S
        };
    };

    hubJobTitle.fn._registerSubscribedHubs = function () {
        /// <summary>
        ///     Sets the starting event to loop through the known hubs and register any new hubs
        ///     that have been added to the proxy.
        /// </summary>
        var JobTitle = this;

        if (!JobTitle._subscribedToHubs) {
            JobTitle._subscribedToHubs = true;
            JobTitle.starting(function () {
                // Set the JobTitle's data object with all the hub proxies with active subscriptions.
                // These proxies will receive notifications from the server.
                var subscribedHubs = [];

                $.each(JobTitle.proxies, function (key) {
                    if (this.hasSubscriptions()) {
                        subscribedHubs.push({ name: key });
                        JobTitle.log("Client subscribed to hub '" + key + "'.");
                    }
                });

                if (subscribedHubs.length === 0) {
                    JobTitle.log("No hubs have been subscribed to.  The client will not receive data from hubs.  To fix, declare at least one client side function prior to JobTitle start for each hub you wish to subscribe to.");
                }

                JobTitle.data = JobTitle.json.stringify(subscribedHubs);
            });
        }
    };

    hubJobTitle.fn.createHubProxy = function (hubName) {
        /// <summary>
        ///     Creates a new proxy object for the given hub JobTitle that can be used to invoke
        ///     methods on server hubs and handle client method invocation requests from the server.
        /// </summary>
        /// <param name="hubName" type="String">
        ///     The name of the hub on the server to create the proxy for.
        /// </param>

        // Normalize the name to lowercase
        hubName = hubName.toLowerCase();

        var proxy = this.proxies[hubName];
        if (!proxy) {
            proxy = hubProxy(this, hubName);
            this.proxies[hubName] = proxy;
        }

        this._registerSubscribedHubs();

        return proxy;
    };

    hubJobTitle.fn.init.prototype = hubJobTitle.fn;

    $.hubJobTitle = hubJobTitle;

}(window.jQuery, window));
/* jquery.signalR.version.js */
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


/*global window:false */
/// <reference path="jquery.signalR.core.js" />
(function ($, undefined) {
    $.signalR.version = "2.2.2";
}(window.jQuery));
