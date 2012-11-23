// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
// Implementation of CallContext ... currently leverages off
// the LocalDataStore facility. 
namespace System.Runtime.Remoting.Messaging{ 

    using System.Threading; 
    using System.Runtime.Remoting;
    using System.Security.Principal;
    using System.Collections;
    using System.Runtime.Serialization; 
    using System.Security.Permissions;
    // This class exposes the API for the users of call context. All methods 
    // in CallContext are static and operate upon the call context in the Thread. 
    // NOTE: CallContext is a specialized form of something that behaves like
    // TLS for method calls. However, since the call objects may get serialized 
    // and deserialized along the path, it is tough to guarantee identity
    // preservation.
    // The LogicalCallContext class has all the actual functionality. We have
    // to use this scheme because Remoting message sinks etc do need to have 
    // the distinction between the call context on the physical thread and
    // the call context that the remoting message actually carries. In most cases 
    // they will operate on the message's call context and hence the latter 
    // exposes the same set of methods as instance methods.
 
    // Only statics does not need to marked with the serializable attribute
    [System.Security.SecurityCritical]  // auto-generated_required
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)] 
    public sealed class CallContext
    { 
        private CallContext() 
        {
        } 

        // Get the logical call context object for the current thread.
        internal static LogicalCallContext GetLogicalCallContext()
        { 
            return Thread.CurrentThread.GetLogicalCallContext();
        } 
 
        // Sets the given logical call context object on the thread.
        // Returns the previous one. 
        internal static LogicalCallContext SetLogicalCallContext(
            LogicalCallContext callCtx)
        {
            return Thread.CurrentThread.SetLogicalCallContext(callCtx); 
        }
 
        internal static LogicalCallContext SetLogicalCallContext( 
            Thread currThread, LogicalCallContext callCtx)
        { 
            return currThread.SetLogicalCallContext(callCtx);
        }

        internal static CallContextSecurityData SecurityData 
        {
            [System.Security.SecurityCritical]  // auto-generated 
            get { return Thread.CurrentThread.GetLogicalCallContext().SecurityData; } 
        }
 
        internal static CallContextRemotingData RemotingData
        {
            [System.Security.SecurityCritical]  // auto-generated
            get { return Thread.CurrentThread.GetLogicalCallContext().RemotingData; } 
        }
 
 
        /*==========================================================================
        ** Frees a named data slot. 
        =========================================================================*/
        [System.Security.SecurityCritical]  // auto-generated
        public static void FreeNamedDataSlot(String name)
        { 
            Thread.CurrentThread.GetLogicalCallContext().FreeNamedDataSlot(name);
            Thread.CurrentThread.GetIllogicalCallContext().FreeNamedDataSlot(name); 
        } 

        /*========================================================================= 
        ** Get data on the logical call context
        =========================================================================*/
        [System.Security.SecurityCritical]  // auto-generated
        public static Object LogicalGetData(String name) 
        {
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            return lcc.GetData(name); 
        }
 
        /*=========================================================================
        ** Get data on the illogical call context
        =========================================================================*/
        private static Object IllogicalGetData(String name) 
        {
            IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
            return ilcc.GetData(name); 
        }
 
        internal static IPrincipal Principal
        {
            [System.Security.SecurityCritical]  // auto-generated
            get 
            {
                LogicalCallContext lcc = GetLogicalCallContext(); 
                return lcc.Principal; 
            }
 
            [System.Security.SecurityCritical]  // auto-generated
            set
            {
                LogicalCallContext lcc = GetLogicalCallContext(); 
                lcc.Principal = value;
            } 
        } 

        public static Object HostContext 
        {
            [System.Security.SecurityCritical]  // auto-generated
            get
            { 
                Object hC;
                IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext(); 
                hC = ilcc.HostContext; 
                if (hC == null)
                { 
                    LogicalCallContext lcc = GetLogicalCallContext();
                    hC = lcc.HostContext;
                }
                return hC; 
            }
            [System.Security.SecurityCritical]  // auto-generated_required 
            set 
            {
                if (value is ILogicalThreadAffinative) 
                {
                    IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext();
                    ilcc.HostContext = null;
                    LogicalCallContext lcc = GetLogicalCallContext(); 
                    lcc.HostContext = value;
                } 
                else 
                {
                    LogicalCallContext lcc = GetLogicalCallContext(); 
                    lcc.HostContext = null;
                    IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext();
                    ilcc.HostContext = value;
                } 
            }
        } 
 
        // <STRIP>For callContexts we intend to expose only name, value dictionary
        // type of behavior for now. We will re-consider if we need to expose 
        // the other functions above for Beta-2.</STRIP>
        [System.Security.SecurityCritical]  // auto-generated
        public static Object GetData(String name)
        { 
            Object o = LogicalGetData(name);
            if (o == null) 
            { 
                return IllogicalGetData(name);
            } 
            else
            {
                return o;
            } 
        }
 
        [System.Security.SecurityCritical]  // auto-generated 
        public static void SetData(String name, Object data)
        { 
            if (data is ILogicalThreadAffinative)
            {
                LogicalSetData(name, data);
            } 
            else
            { 
                LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
                lcc.FreeNamedDataSlot(name);
                IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
                ilcc.SetData(name, data);
            }
        }
        [System.Security.SecurityCritical]  // auto-generated 
        public static void LogicalSetData(String name, Object data)
        { 
            IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
            ilcc.FreeNamedDataSlot(name);
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            lcc.SetData(name, data);
        }

 
        [System.Security.SecurityCritical]  // auto-generated
        public static Header[] GetHeaders() 
        { 
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext();
            return lcc.InternalGetHeaders(); 
        } // GetHeaders

        [System.Security.SecurityCritical]  // auto-generated
        public static void SetHeaders(Header[] headers) 
        {
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            lcc.InternalSetHeaders(headers); 
        } // SetHeaders
 
    } // class CallContext

[System.Runtime.InteropServices.ComVisible(true)]
    public interface ILogicalThreadAffinative 
    {
    } 
 
    internal class IllogicalCallContext: ICloneable
    { 
        private Hashtable m_Datastore;
        private Object m_HostContext;

        private Hashtable Datastore 
        {
            get 
            { 
                if (null == m_Datastore)
                { 
                    // The local store has not yet been created for this thread.
                    m_Datastore = new Hashtable();
                }
                return m_Datastore; 
            }
        } 
 
        internal Object HostContext
        { 
            get
            {
                return m_HostContext;
            } 
            set
            { 
                m_HostContext = value; 
            }
        } 

        internal bool HasUserData
        {
            get { return ((m_Datastore != null) && (m_Datastore.Count > 0));} 
        }
 
        /*========================================================================= 
        ** Frees a named data slot.
        =========================================================================*/ 
        public void FreeNamedDataSlot(String name)
        {
            Datastore.Remove(name);
        } 

        public Object GetData(String name) 
        { 
            return Datastore[name];
        } 

        public void SetData(String name, Object data)
        {
            Datastore[name] = data; 
        }
 
        public Object Clone() 
        {
            IllogicalCallContext ilcc = new IllogicalCallContext(); 
            if (HasUserData)
            {
                IDictionaryEnumerator de = m_Datastore.GetEnumerator();
 
                while (de.MoveNext())
                { 
                    ilcc.Datastore[(String)de.Key] = de.Value; 
                }
            } 
            return ilcc;
        }
    }
 
    // This class handles the actual call context functionality. It leverages on the
    // implementation of local data store ... except that the local store manager is 
    // not static. That is to say, allocating a slot in one call context has no effect 
    // on another call contexts. Different call contexts are entirely unrelated.
 
    [System.Security.SecurityCritical]  // auto-generated_required
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class LogicalCallContext : ISerializable, ICloneable 
    {
        // Private static data 
        private static Type s_callContextType = typeof(LogicalCallContext); 
        private const string s_CorrelationMgrSlotName = "System.Diagnostics.Trace.CorrelationManagerSlot";
 
        /*==========================================================================
        ** Data accessed from managed code that needs to be defined in
        ** LogicalCallContextObject  to maintain alignment between the two classes.
        ** DON'T CHANGE THESE UNLESS YOU MODIFY LogicalContextObject in vm\object.h 
        =========================================================================*/
 
        // Private member data 
        private Hashtable m_Datastore;
        private CallContextRemotingData m_RemotingData = null; 
        private CallContextSecurityData m_SecurityData = null;
        private Object m_HostContext = null;
        private bool m_IsCorrelationMgr = false;
 
        // _sendHeaders is for Headers that should be sent out on the next call.
        // _recvHeaders are for Headers that came from a response. 
        private Header[] _sendHeaders = null; 
        private Header[] _recvHeaders = null;
 

        internal LogicalCallContext()
        {
        } 

        [System.Security.SecurityCritical]  // auto-generated 
        internal LogicalCallContext(SerializationInfo info, StreamingContext context) 
        {
            SerializationInfoEnumerator e = info.GetEnumerator(); 
            while (e.MoveNext())
            {
                if (e.Name.Equals("__RemotingData"))
                { 
                    m_RemotingData = (CallContextRemotingData) e.Value;
                } 
                else if (e.Name.Equals("__SecurityData")) 
                {
                    if (context.State == StreamingContextStates.CrossAppDomain) 
                    {
                        m_SecurityData = (CallContextSecurityData) e.Value;
                    }
                    else 
                    {
                        BCLDebug.Assert(false, "Security data should only be serialized in cross appdomain case."); 
                    } 
                }
                else if (e.Name.Equals("__HostContext")) 
                {
                    m_HostContext = e.Value;
                }
                else if (e.Name.Equals("__CorrelationMgrSlotPresent")) 
                {
                    m_IsCorrelationMgr = (bool)e.Value; 
                } 
                else
                { 
                    Datastore[e.Name] = e.Value;
                }

            } 
        }
 
        [System.Security.SecurityCritical]  // auto-generated_required 
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        { 
            if (info == null)
                throw new ArgumentNullException("info");
            info.SetType(s_callContextType);
            if (m_RemotingData != null) 
            {
                info.AddValue("__RemotingData", m_RemotingData); 
            } 
            if (m_SecurityData != null)
            { 
                if (context.State == StreamingContextStates.CrossAppDomain)
                {
                    info.AddValue("__SecurityData", m_SecurityData);
                } 
            }
            if (m_HostContext != null) 
            { 
                info.AddValue("__HostContext", m_HostContext);
            } 
            if (m_IsCorrelationMgr)
            {
                info.AddValue("__CorrelationMgrSlotPresent", m_IsCorrelationMgr);
            } 
            if (HasUserData)
            { 
                IDictionaryEnumerator de = m_Datastore.GetEnumerator(); 

                while (de.MoveNext()) 
                {
                    info.AddValue((String)de.Key, de.Value);
                }
            } 

        } 
 

        // ICloneable::Clone 
        // Used to create a deep copy of the call context when an async
        // call starts.

        // < 

 
 
        [System.Security.SecuritySafeCritical] // overrides public transparent member
        public Object Clone() 
        {
            LogicalCallContext lc = new LogicalCallContext();
            if (m_RemotingData != null)
                lc.m_RemotingData = (CallContextRemotingData)m_RemotingData.Clone(); 
            if (m_SecurityData != null)
                lc.m_SecurityData = (CallContextSecurityData)m_SecurityData.Clone(); 
            if (m_HostContext != null) 
                lc.m_HostContext = m_HostContext;
            if (HasUserData) 
            {
                IDictionaryEnumerator de = m_Datastore.GetEnumerator();

                if (!m_IsCorrelationMgr) 
                {
                    while (de.MoveNext()) 
                    { 
                        lc.Datastore[(String)de.Key] = de.Value;
                    } 
                }
                else
                {
                    while (de.MoveNext()) 
                    {
                        String key = (String)de.Key; 
 
                        // Deep clone "System.Diagnostics.Trace.CorrelationManagerSlot"
                        if (key.Equals(s_CorrelationMgrSlotName)) 
                        {
                            lc.Datastore[key] = ((ICloneable)de.Value).Clone();
                        }
                        else 
                            lc.Datastore[key] = de.Value;
                    } 
                } 
            }
            return lc; 
        }

        // Used to do a (limited) merge the call context from a returning async call
        [System.Security.SecurityCritical]  // auto-generated 
        internal void Merge(LogicalCallContext lc)
        { 
            // we ignore the RemotingData & SecurityData 
            // and only merge the user sections of the two call contexts
            // the idea being that if the original call had any 
            // identity/remoting callID that should remain unchanged

            // If we have a non-null callContext and it is not the same
            // as the one on the current thread (can happen in x-context async) 
            // and there is any userData in the callContext, do the merge
            if ((lc != null) && (this != lc) && lc.HasUserData) 
            { 
                IDictionaryEnumerator de = lc.Datastore.GetEnumerator();
 
                while (de.MoveNext())
                {
                    Datastore[(String)de.Key] = de.Value;
                } 
            }
        } 
 
        public bool HasInfo
        { 
            [System.Security.SecurityCritical]  // auto-generated
            get
            {
                bool fInfo = false; 

                // Set the flag to true if there is either remoting data, or 
                // security data or user data 
                if(
                    (m_RemotingData != null &&  m_RemotingData.HasInfo) || 
                    (m_SecurityData != null &&  m_SecurityData.HasInfo) ||
                    (m_HostContext != null) ||
                    HasUserData
                  ) 
                {
                    fInfo = true; 
                } 

                return fInfo; 
            }
        }

        private bool HasUserData 
        {
            get { return ((m_Datastore != null) && (m_Datastore.Count > 0));} 
        } 

        internal CallContextRemotingData RemotingData 
        {
            get
            {
                if (m_RemotingData == null) 
                    m_RemotingData = new CallContextRemotingData();
 
                return m_RemotingData; 
            }
        } 

        internal CallContextSecurityData SecurityData
        {
            get 
            {
                if (m_SecurityData == null) 
                    m_SecurityData = new CallContextSecurityData(); 

                return m_SecurityData; 
            }
        }

        internal Object HostContext 
        {
            get 
            { 
                return m_HostContext;
            } 
            set
            {
                m_HostContext = value;
            } 
        }
 
        private Hashtable Datastore 
        {
            get 
            {
                if (null == m_Datastore)
                {
                    // The local store has not yet been created for this thread. 
                    m_Datastore = new Hashtable();
                } 
                return m_Datastore; 
            }
        } 

        // This is used for quick access to the current principal when going
        // between appdomains.
        internal IPrincipal Principal 
        {
            get 
            { 
                // This MUST not fault in the security data object if it doesn't exist.
                if (m_SecurityData != null) 
                    return m_SecurityData.Principal;

                return null;
            } // get 

            [System.Security.SecurityCritical]  // auto-generated 
            set 
            {
                SecurityData.Principal = value; 
            } // set
        } // Principal

        /*========================================================================= 
        ** Frees a named data slot.
        =========================================================================*/ 
        [System.Security.SecurityCritical]  // auto-generated 
        public void FreeNamedDataSlot(String name)
        { 
            Datastore.Remove(name);
        }

        [System.Security.SecurityCritical]  // auto-generated 
        public Object GetData(String name)
        { 
            return Datastore[name]; 
        }
 
        [System.Security.SecurityCritical]  // auto-generated
        public void SetData(String name, Object data)
        {
            Datastore[name] = data; 
            if (name.Equals(s_CorrelationMgrSlotName))
                m_IsCorrelationMgr = true; 
        } 

        private Header[] InternalGetOutgoingHeaders() 
        {
            Header[] outgoingHeaders = _sendHeaders;
            _sendHeaders = null;
 
            // A new remote call is being made, so we null out the
            //   current received headers so these can't be confused 
            //   with a response from the next call. 
            _recvHeaders = null;
 
            return outgoingHeaders;
        } // InternalGetOutgoingHeaders

 
        internal void InternalSetHeaders(Header[] headers)
        { 
            _sendHeaders = headers; 
            _recvHeaders = null;
        } // InternalSetHeaders 


        internal Header[] InternalGetHeaders()
        { 
            // If _sendHeaders is currently set, we always want to return them.
            if (_sendHeaders != null) 
                return _sendHeaders; 

            // Either _recvHeaders is non-null and those are the ones we want to 
            //   return, or there are no currently set headers, so we'll return
            //    null.
            return _recvHeaders;
        } // InternalGetHeaders 

        // Nulls out the principal if its not serializable. 
        // Since principals do flow for x-appdomain cases 
        // we need to handle this behaviour both during invoke
        // and response 
        [System.Security.SecurityCritical]  // auto-generated
        internal IPrincipal RemovePrincipalIfNotSerializable()
        {
            IPrincipal currentPrincipal = this.Principal; 
            // If the principal is not serializable, we need to
            //   null it out. 
            if (currentPrincipal != null) 
            {
                if (!currentPrincipal.GetType().IsSerializable) 
                    this.Principal = null;
            }
            return currentPrincipal;
        } 
#if FEATURE_REMOTING
        // Takes outgoing headers and inserts them 
        [System.Security.SecurityCritical]  // auto-generated 
        internal void PropagateOutgoingHeadersToMessage(IMessage msg)
        { 
            Header[] headers = InternalGetOutgoingHeaders();

            if (headers != null)
            { 
                BCLDebug.Assert(msg != null, "Why is the message null?");
 
                IDictionary properties = msg.Properties; 
                BCLDebug.Assert(properties != null, "Why are the properties null?");
 
                foreach (Header header in headers)
                {
                    // add header to the message dictionary
                    if (header != null) 
                    {
                        // The header key is composed from its name and namespace. 
 
                        String name = GetPropertyKeyForHeader(header);
 
                        properties[name] = header;
                    }
                }
            } 
        } // PropagateOutgoingHeadersToMessage
#endif 
        // Retrieve key to use for header. 
        internal static String GetPropertyKeyForHeader(Header header)
        { 
            if (header == null)
                return null;

            if (header.HeaderNamespace != null) 
                return header.Name + ", " + header.HeaderNamespace;
            else 
                return header.Name; 
        } // GetPropertyKeyForHeader
 
#if FEATURE_REMOTING
        // Take headers out of message and stores them in call context
        [System.Security.SecurityCritical]  // auto-generated
        internal void PropagateIncomingHeadersToCallContext(IMessage msg) 
        {
            BCLDebug.Assert(msg != null, "Why is the message null?"); 
 
            // If it's an internal message, we can quickly tell if there are any
            //   headers. 
            IInternalMessage iim = msg as IInternalMessage;
            if (iim != null)
            {
                if (!iim.HasProperties()) 
                {
                    // If there are no properties just return immediately. 
                    return; 
                }
            } 

            IDictionary properties = msg.Properties;
            BCLDebug.Assert(properties != null, "Why are the properties null?");
 
            IDictionaryEnumerator e = (IDictionaryEnumerator) properties.GetEnumerator();
 
            // cycle through the properties to get a count of the headers 
            int count = 0;
            while (e.MoveNext()) 
            {
                String key = (String)e.Key;
                if (!key.StartsWith("__", StringComparison.Ordinal))
                { 
                    // We don't want to have to check for special values, so we
                    //   blanketly state that header names can't start with 
                    //   double underscore. 
                    if (e.Value is Header)
                        count++; 
                }
            }

            // If there are headers, create array and set it to the received header property 
            Header[] headers = null;
            if (count > 0) 
            { 
                headers = new Header[count];
 
                count = 0;
                e.Reset();
                while (e.MoveNext())
                { 
                    String key = (String)e.Key;
                    if (!key.StartsWith("__", StringComparison.Ordinal)) 
                    { 
                        Header header = e.Value as Header;
                        if (header != null) 
                            headers[count++] = header;
                    }
                }
            } 

            _recvHeaders = headers; 
            _sendHeaders = null; 
        } // PropagateIncomingHeadersToCallContext
#endif // FEATURE_REMOTING 
    } // class LogicalCallContext


 
    [Serializable]
    internal class CallContextSecurityData : ICloneable 
    { 
        // This is used for the special getter/setter for security related
        // info in the callContext. 
        IPrincipal _principal;
        // <
        internal IPrincipal Principal
        { 
            get {return _principal;}
            set {_principal = value;} 
        } 

        // Checks if there is any useful data to be serialized 
        internal bool HasInfo
        {
            get
            { 
                return (null != _principal);
            } 
 
        }
 
        public Object Clone()
        {
            CallContextSecurityData sd = new CallContextSecurityData();
            sd._principal = _principal; 
            return sd;
        } 
 
    }
 
    [Serializable]
    internal class CallContextRemotingData : ICloneable
    {
        // This is used for the special getter/setter for remoting related 
        // info in the callContext.
        String _logicalCallID; 
 
        internal String LogicalCallID
        { 
            get  {return _logicalCallID;}
            set  {_logicalCallID = value;}
        }
 
        // Checks if there is any useful data to be serialized
        internal bool HasInfo 
        { 
            get
            { 
                // Keep this updated if we add more stuff to remotingData!
                return (_logicalCallID!=null);
            }
        } 

        public Object Clone() 
        { 
            CallContextRemotingData rd = new CallContextRemotingData();
            rd.LogicalCallID = LogicalCallID; 
            return rd;
        }
    }
 } 

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
// Implementation of CallContext ... currently leverages off
// the LocalDataStore facility. 
namespace System.Runtime.Remoting.Messaging{ 

    using System.Threading; 
    using System.Runtime.Remoting;
    using System.Security.Principal;
    using System.Collections;
    using System.Runtime.Serialization; 
    using System.Security.Permissions;
    // This class exposes the API for the users of call context. All methods 
    // in CallContext are static and operate upon the call context in the Thread. 
    // NOTE: CallContext is a specialized form of something that behaves like
    // TLS for method calls. However, since the call objects may get serialized 
    // and deserialized along the path, it is tough to guarantee identity
    // preservation.
    // The LogicalCallContext class has all the actual functionality. We have
    // to use this scheme because Remoting message sinks etc do need to have 
    // the distinction between the call context on the physical thread and
    // the call context that the remoting message actually carries. In most cases 
    // they will operate on the message's call context and hence the latter 
    // exposes the same set of methods as instance methods.
 
    // Only statics does not need to marked with the serializable attribute
    [System.Security.SecurityCritical]  // auto-generated_required
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)] 
    public sealed class CallContext
    { 
        private CallContext() 
        {
        } 

        // Get the logical call context object for the current thread.
        internal static LogicalCallContext GetLogicalCallContext()
        { 
            return Thread.CurrentThread.GetLogicalCallContext();
        } 
 
        // Sets the given logical call context object on the thread.
        // Returns the previous one. 
        internal static LogicalCallContext SetLogicalCallContext(
            LogicalCallContext callCtx)
        {
            return Thread.CurrentThread.SetLogicalCallContext(callCtx); 
        }
 
        internal static LogicalCallContext SetLogicalCallContext( 
            Thread currThread, LogicalCallContext callCtx)
        { 
            return currThread.SetLogicalCallContext(callCtx);
        }

        internal static CallContextSecurityData SecurityData 
        {
            [System.Security.SecurityCritical]  // auto-generated 
            get { return Thread.CurrentThread.GetLogicalCallContext().SecurityData; } 
        }
 
        internal static CallContextRemotingData RemotingData
        {
            [System.Security.SecurityCritical]  // auto-generated
            get { return Thread.CurrentThread.GetLogicalCallContext().RemotingData; } 
        }
 
 
        /*==========================================================================
        ** Frees a named data slot. 
        =========================================================================*/
        [System.Security.SecurityCritical]  // auto-generated
        public static void FreeNamedDataSlot(String name)
        { 
            Thread.CurrentThread.GetLogicalCallContext().FreeNamedDataSlot(name);
            Thread.CurrentThread.GetIllogicalCallContext().FreeNamedDataSlot(name); 
        } 

        /*========================================================================= 
        ** Get data on the logical call context
        =========================================================================*/
        [System.Security.SecurityCritical]  // auto-generated
        public static Object LogicalGetData(String name) 
        {
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            return lcc.GetData(name); 
        }
 
        /*=========================================================================
        ** Get data on the illogical call context
        =========================================================================*/
        private static Object IllogicalGetData(String name) 
        {
            IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
            return ilcc.GetData(name); 
        }
 
        internal static IPrincipal Principal
        {
            [System.Security.SecurityCritical]  // auto-generated
            get 
            {
                LogicalCallContext lcc = GetLogicalCallContext(); 
                return lcc.Principal; 
            }
 
            [System.Security.SecurityCritical]  // auto-generated
            set
            {
                LogicalCallContext lcc = GetLogicalCallContext(); 
                lcc.Principal = value;
            } 
        } 

        public static Object HostContext 
        {
            [System.Security.SecurityCritical]  // auto-generated
            get
            { 
                Object hC;
                IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext(); 
                hC = ilcc.HostContext; 
                if (hC == null)
                { 
                    LogicalCallContext lcc = GetLogicalCallContext();
                    hC = lcc.HostContext;
                }
                return hC; 
            }
            [System.Security.SecurityCritical]  // auto-generated_required 
            set 
            {
                if (value is ILogicalThreadAffinative) 
                {
                    IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext();
                    ilcc.HostContext = null;
                    LogicalCallContext lcc = GetLogicalCallContext(); 
                    lcc.HostContext = value;
                } 
                else 
                {
                    LogicalCallContext lcc = GetLogicalCallContext(); 
                    lcc.HostContext = null;
                    IllogicalCallContext ilcc = Thread.CurrentThread.GetIllogicalCallContext();
                    ilcc.HostContext = value;
                } 
            }
        } 
 
        // <STRIP>For callContexts we intend to expose only name, value dictionary
        // type of behavior for now. We will re-consider if we need to expose 
        // the other functions above for Beta-2.</STRIP>
        [System.Security.SecurityCritical]  // auto-generated
        public static Object GetData(String name)
        { 
            Object o = LogicalGetData(name);
            if (o == null) 
            { 
                return IllogicalGetData(name);
            } 
            else
            {
                return o;
            } 
        }
 
        [System.Security.SecurityCritical]  // auto-generated 
        public static void SetData(String name, Object data)
        { 
            if (data is ILogicalThreadAffinative)
            {
                LogicalSetData(name, data);
            } 
            else
            { 
                LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
                lcc.FreeNamedDataSlot(name);
                IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
                ilcc.SetData(name, data);
            }
        }
        [System.Security.SecurityCritical]  // auto-generated 
        public static void LogicalSetData(String name, Object data)
        { 
            IllogicalCallContext ilcc =  Thread.CurrentThread.GetIllogicalCallContext(); 
            ilcc.FreeNamedDataSlot(name);
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            lcc.SetData(name, data);
        }

 
        [System.Security.SecurityCritical]  // auto-generated
        public static Header[] GetHeaders() 
        { 
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext();
            return lcc.InternalGetHeaders(); 
        } // GetHeaders

        [System.Security.SecurityCritical]  // auto-generated
        public static void SetHeaders(Header[] headers) 
        {
            LogicalCallContext lcc =  Thread.CurrentThread.GetLogicalCallContext(); 
            lcc.InternalSetHeaders(headers); 
        } // SetHeaders
 
    } // class CallContext

[System.Runtime.InteropServices.ComVisible(true)]
    public interface ILogicalThreadAffinative 
    {
    } 
 
    internal class IllogicalCallContext: ICloneable
    { 
        private Hashtable m_Datastore;
        private Object m_HostContext;

        private Hashtable Datastore 
        {
            get 
            { 
                if (null == m_Datastore)
                { 
                    // The local store has not yet been created for this thread.
                    m_Datastore = new Hashtable();
                }
                return m_Datastore; 
            }
        } 
 
        internal Object HostContext
        { 
            get
            {
                return m_HostContext;
            } 
            set
            { 
                m_HostContext = value; 
            }
        } 

        internal bool HasUserData
        {
            get { return ((m_Datastore != null) && (m_Datastore.Count > 0));} 
        }
 
        /*========================================================================= 
        ** Frees a named data slot.
        =========================================================================*/ 
        public void FreeNamedDataSlot(String name)
        {
            Datastore.Remove(name);
        } 

        public Object GetData(String name) 
        { 
            return Datastore[name];
        } 

        public void SetData(String name, Object data)
        {
            Datastore[name] = data; 
        }
 
        public Object Clone() 
        {
            IllogicalCallContext ilcc = new IllogicalCallContext(); 
            if (HasUserData)
            {
                IDictionaryEnumerator de = m_Datastore.GetEnumerator();
 
                while (de.MoveNext())
                { 
                    ilcc.Datastore[(String)de.Key] = de.Value; 
                }
            } 
            return ilcc;
        }
    }
 
    // This class handles the actual call context functionality. It leverages on the
    // implementation of local data store ... except that the local store manager is 
    // not static. That is to say, allocating a slot in one call context has no effect 
    // on another call contexts. Different call contexts are entirely unrelated.
 
    [System.Security.SecurityCritical]  // auto-generated_required
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class LogicalCallContext : ISerializable, ICloneable 
    {
        // Private static data 
        private static Type s_callContextType = typeof(LogicalCallContext); 
        private const string s_CorrelationMgrSlotName = "System.Diagnostics.Trace.CorrelationManagerSlot";
 
        /*==========================================================================
        ** Data accessed from managed code that needs to be defined in
        ** LogicalCallContextObject  to maintain alignment between the two classes.
        ** DON'T CHANGE THESE UNLESS YOU MODIFY LogicalContextObject in vm\object.h 
        =========================================================================*/
 
        // Private member data 
        private Hashtable m_Datastore;
        private CallContextRemotingData m_RemotingData = null; 
        private CallContextSecurityData m_SecurityData = null;
        private Object m_HostContext = null;
        private bool m_IsCorrelationMgr = false;
 
        // _sendHeaders is for Headers that should be sent out on the next call.
        // _recvHeaders are for Headers that came from a response. 
        private Header[] _sendHeaders = null; 
        private Header[] _recvHeaders = null;
 

        internal LogicalCallContext()
        {
        } 

        [System.Security.SecurityCritical]  // auto-generated 
        internal LogicalCallContext(SerializationInfo info, StreamingContext context) 
        {
            SerializationInfoEnumerator e = info.GetEnumerator(); 
            while (e.MoveNext())
            {
                if (e.Name.Equals("__RemotingData"))
                { 
                    m_RemotingData = (CallContextRemotingData) e.Value;
                } 
                else if (e.Name.Equals("__SecurityData")) 
                {
                    if (context.State == StreamingContextStates.CrossAppDomain) 
                    {
                        m_SecurityData = (CallContextSecurityData) e.Value;
                    }
                    else 
                    {
                        BCLDebug.Assert(false, "Security data should only be serialized in cross appdomain case."); 
                    } 
                }
                else if (e.Name.Equals("__HostContext")) 
                {
                    m_HostContext = e.Value;
                }
                else if (e.Name.Equals("__CorrelationMgrSlotPresent")) 
                {
                    m_IsCorrelationMgr = (bool)e.Value; 
                } 
                else
                { 
                    Datastore[e.Name] = e.Value;
                }

            } 
        }
 
        [System.Security.SecurityCritical]  // auto-generated_required 
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        { 
            if (info == null)
                throw new ArgumentNullException("info");
            info.SetType(s_callContextType);
            if (m_RemotingData != null) 
            {
                info.AddValue("__RemotingData", m_RemotingData); 
            } 
            if (m_SecurityData != null)
            { 
                if (context.State == StreamingContextStates.CrossAppDomain)
                {
                    info.AddValue("__SecurityData", m_SecurityData);
                } 
            }
            if (m_HostContext != null) 
            { 
                info.AddValue("__HostContext", m_HostContext);
            } 
            if (m_IsCorrelationMgr)
            {
                info.AddValue("__CorrelationMgrSlotPresent", m_IsCorrelationMgr);
            } 
            if (HasUserData)
            { 
                IDictionaryEnumerator de = m_Datastore.GetEnumerator(); 

                while (de.MoveNext()) 
                {
                    info.AddValue((String)de.Key, de.Value);
                }
            } 

        } 
 

        // ICloneable::Clone 
        // Used to create a deep copy of the call context when an async
        // call starts.

        // < 

 
 
        [System.Security.SecuritySafeCritical] // overrides public transparent member
        public Object Clone() 
        {
            LogicalCallContext lc = new LogicalCallContext();
            if (m_RemotingData != null)
                lc.m_RemotingData = (CallContextRemotingData)m_RemotingData.Clone(); 
            if (m_SecurityData != null)
                lc.m_SecurityData = (CallContextSecurityData)m_SecurityData.Clone(); 
            if (m_HostContext != null) 
                lc.m_HostContext = m_HostContext;
            if (HasUserData) 
            {
                IDictionaryEnumerator de = m_Datastore.GetEnumerator();

                if (!m_IsCorrelationMgr) 
                {
                    while (de.MoveNext()) 
                    { 
                        lc.Datastore[(String)de.Key] = de.Value;
                    } 
                }
                else
                {
                    while (de.MoveNext()) 
                    {
                        String key = (String)de.Key; 
 
                        // Deep clone "System.Diagnostics.Trace.CorrelationManagerSlot"
                        if (key.Equals(s_CorrelationMgrSlotName)) 
                        {
                            lc.Datastore[key] = ((ICloneable)de.Value).Clone();
                        }
                        else 
                            lc.Datastore[key] = de.Value;
                    } 
                } 
            }
            return lc; 
        }

        // Used to do a (limited) merge the call context from a returning async call
        [System.Security.SecurityCritical]  // auto-generated 
        internal void Merge(LogicalCallContext lc)
        { 
            // we ignore the RemotingData & SecurityData 
            // and only merge the user sections of the two call contexts
            // the idea being that if the original call had any 
            // identity/remoting callID that should remain unchanged

            // If we have a non-null callContext and it is not the same
            // as the one on the current thread (can happen in x-context async) 
            // and there is any userData in the callContext, do the merge
            if ((lc != null) && (this != lc) && lc.HasUserData) 
            { 
                IDictionaryEnumerator de = lc.Datastore.GetEnumerator();
 
                while (de.MoveNext())
                {
                    Datastore[(String)de.Key] = de.Value;
                } 
            }
        } 
 
        public bool HasInfo
        { 
            [System.Security.SecurityCritical]  // auto-generated
            get
            {
                bool fInfo = false; 

                // Set the flag to true if there is either remoting data, or 
                // security data or user data 
                if(
                    (m_RemotingData != null &&  m_RemotingData.HasInfo) || 
                    (m_SecurityData != null &&  m_SecurityData.HasInfo) ||
                    (m_HostContext != null) ||
                    HasUserData
                  ) 
                {
                    fInfo = true; 
                } 

                return fInfo; 
            }
        }

        private bool HasUserData 
        {
            get { return ((m_Datastore != null) && (m_Datastore.Count > 0));} 
        } 

        internal CallContextRemotingData RemotingData 
        {
            get
            {
                if (m_RemotingData == null) 
                    m_RemotingData = new CallContextRemotingData();
 
                return m_RemotingData; 
            }
        } 

        internal CallContextSecurityData SecurityData
        {
            get 
            {
                if (m_SecurityData == null) 
                    m_SecurityData = new CallContextSecurityData(); 

                return m_SecurityData; 
            }
        }

        internal Object HostContext 
        {
            get 
            { 
                return m_HostContext;
            } 
            set
            {
                m_HostContext = value;
            } 
        }
 
        private Hashtable Datastore 
        {
            get 
            {
                if (null == m_Datastore)
                {
                    // The local store has not yet been created for this thread. 
                    m_Datastore = new Hashtable();
                } 
                return m_Datastore; 
            }
        } 

        // This is used for quick access to the current principal when going
        // between appdomains.
        internal IPrincipal Principal 
        {
            get 
            { 
                // This MUST not fault in the security data object if it doesn't exist.
                if (m_SecurityData != null) 
                    return m_SecurityData.Principal;

                return null;
            } // get 

            [System.Security.SecurityCritical]  // auto-generated 
            set 
            {
                SecurityData.Principal = value; 
            } // set
        } // Principal

        /*========================================================================= 
        ** Frees a named data slot.
        =========================================================================*/ 
        [System.Security.SecurityCritical]  // auto-generated 
        public void FreeNamedDataSlot(String name)
        { 
            Datastore.Remove(name);
        }

        [System.Security.SecurityCritical]  // auto-generated 
        public Object GetData(String name)
        { 
            return Datastore[name]; 
        }
 
        [System.Security.SecurityCritical]  // auto-generated
        public void SetData(String name, Object data)
        {
            Datastore[name] = data; 
            if (name.Equals(s_CorrelationMgrSlotName))
                m_IsCorrelationMgr = true; 
        } 

        private Header[] InternalGetOutgoingHeaders() 
        {
            Header[] outgoingHeaders = _sendHeaders;
            _sendHeaders = null;
 
            // A new remote call is being made, so we null out the
            //   current received headers so these can't be confused 
            //   with a response from the next call. 
            _recvHeaders = null;
 
            return outgoingHeaders;
        } // InternalGetOutgoingHeaders

 
        internal void InternalSetHeaders(Header[] headers)
        { 
            _sendHeaders = headers; 
            _recvHeaders = null;
        } // InternalSetHeaders 


        internal Header[] InternalGetHeaders()
        { 
            // If _sendHeaders is currently set, we always want to return them.
            if (_sendHeaders != null) 
                return _sendHeaders; 

            // Either _recvHeaders is non-null and those are the ones we want to 
            //   return, or there are no currently set headers, so we'll return
            //    null.
            return _recvHeaders;
        } // InternalGetHeaders 

        // Nulls out the principal if its not serializable. 
        // Since principals do flow for x-appdomain cases 
        // we need to handle this behaviour both during invoke
        // and response 
        [System.Security.SecurityCritical]  // auto-generated
        internal IPrincipal RemovePrincipalIfNotSerializable()
        {
            IPrincipal currentPrincipal = this.Principal; 
            // If the principal is not serializable, we need to
            //   null it out. 
            if (currentPrincipal != null) 
            {
                if (!currentPrincipal.GetType().IsSerializable) 
                    this.Principal = null;
            }
            return currentPrincipal;
        } 
#if FEATURE_REMOTING
        // Takes outgoing headers and inserts them 
        [System.Security.SecurityCritical]  // auto-generated 
        internal void PropagateOutgoingHeadersToMessage(IMessage msg)
        { 
            Header[] headers = InternalGetOutgoingHeaders();

            if (headers != null)
            { 
                BCLDebug.Assert(msg != null, "Why is the message null?");
 
                IDictionary properties = msg.Properties; 
                BCLDebug.Assert(properties != null, "Why are the properties null?");
 
                foreach (Header header in headers)
                {
                    // add header to the message dictionary
                    if (header != null) 
                    {
                        // The header key is composed from its name and namespace. 
 
                        String name = GetPropertyKeyForHeader(header);
 
                        properties[name] = header;
                    }
                }
            } 
        } // PropagateOutgoingHeadersToMessage
#endif 
        // Retrieve key to use for header. 
        internal static String GetPropertyKeyForHeader(Header header)
        { 
            if (header == null)
                return null;

            if (header.HeaderNamespace != null) 
                return header.Name + ", " + header.HeaderNamespace;
            else 
                return header.Name; 
        } // GetPropertyKeyForHeader
 
#if FEATURE_REMOTING
        // Take headers out of message and stores them in call context
        [System.Security.SecurityCritical]  // auto-generated
        internal void PropagateIncomingHeadersToCallContext(IMessage msg) 
        {
            BCLDebug.Assert(msg != null, "Why is the message null?"); 
 
            // If it's an internal message, we can quickly tell if there are any
            //   headers. 
            IInternalMessage iim = msg as IInternalMessage;
            if (iim != null)
            {
                if (!iim.HasProperties()) 
                {
                    // If there are no properties just return immediately. 
                    return; 
                }
            } 

            IDictionary properties = msg.Properties;
            BCLDebug.Assert(properties != null, "Why are the properties null?");
 
            IDictionaryEnumerator e = (IDictionaryEnumerator) properties.GetEnumerator();
 
            // cycle through the properties to get a count of the headers 
            int count = 0;
            while (e.MoveNext()) 
            {
                String key = (String)e.Key;
                if (!key.StartsWith("__", StringComparison.Ordinal))
                { 
                    // We don't want to have to check for special values, so we
                    //   blanketly state that header names can't start with 
                    //   double underscore. 
                    if (e.Value is Header)
                        count++; 
                }
            }

            // If there are headers, create array and set it to the received header property 
            Header[] headers = null;
            if (count > 0) 
            { 
                headers = new Header[count];
 
                count = 0;
                e.Reset();
                while (e.MoveNext())
                { 
                    String key = (String)e.Key;
                    if (!key.StartsWith("__", StringComparison.Ordinal)) 
                    { 
                        Header header = e.Value as Header;
                        if (header != null) 
                            headers[count++] = header;
                    }
                }
            } 

            _recvHeaders = headers; 
            _sendHeaders = null; 
        } // PropagateIncomingHeadersToCallContext
#endif // FEATURE_REMOTING 
    } // class LogicalCallContext


 
    [Serializable]
    internal class CallContextSecurityData : ICloneable 
    { 
        // This is used for the special getter/setter for security related
        // info in the callContext. 
        IPrincipal _principal;
        // <
        internal IPrincipal Principal
        { 
            get {return _principal;}
            set {_principal = value;} 
        } 

        // Checks if there is any useful data to be serialized 
        internal bool HasInfo
        {
            get
            { 
                return (null != _principal);
            } 
 
        }
 
        public Object Clone()
        {
            CallContextSecurityData sd = new CallContextSecurityData();
            sd._principal = _principal; 
            return sd;
        } 
 
    }
 
    [Serializable]
    internal class CallContextRemotingData : ICloneable
    {
        // This is used for the special getter/setter for remoting related 
        // info in the callContext.
        String _logicalCallID; 
 
        internal String LogicalCallID
        { 
            get  {return _logicalCallID;}
            set  {_logicalCallID = value;}
        }
 
        // Checks if there is any useful data to be serialized
        internal bool HasInfo 
        { 
            get
            { 
                // Keep this updated if we add more stuff to remotingData!
                return (_logicalCallID!=null);
            }
        } 

        public Object Clone() 
        { 
            CallContextRemotingData rd = new CallContextRemotingData();
            rd.LogicalCallID = LogicalCallID; 
            return rd;
        }
    }
 } 

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
