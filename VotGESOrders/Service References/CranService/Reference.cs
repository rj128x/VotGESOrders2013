﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.34209
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace VotGESOrders.CranService {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CranFilter", Namespace="http://schemas.datacontract.org/2004/07/VotGESOrders.Web.Models")]
    public partial class CranFilter : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.Collections.ObjectModel.ObservableCollection<VotGESOrders.CranService.CranTaskInfo> DataField;
        
        private System.DateTime DateEndField;
        
        private System.DateTime DateStartField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<VotGESOrders.CranService.CranTaskInfo> Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateEnd {
            get {
                return this.DateEndField;
            }
            set {
                if ((this.DateEndField.Equals(value) != true)) {
                    this.DateEndField = value;
                    this.RaisePropertyChanged("DateEnd");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateStart {
            get {
                return this.DateStartField;
            }
            set {
                if ((this.DateStartField.Equals(value) != true)) {
                    this.DateStartField = value;
                    this.RaisePropertyChanged("DateStart");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CranTaskInfo", Namespace="http://schemas.datacontract.org/2004/07/VotGESOrders.Web.Models")]
    public partial class CranTaskInfo : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.DateTime AllowDateEndField;
        
        private System.DateTime AllowDateStartField;
        
        private bool AllowedField;
        
        private string AuthorField;
        
        private string AuthorAllowField;
        
        private string CommentField;
        
        private int CranNumberField;
        
        private bool DeniedField;
        
        private System.DateTime NeedEndDateField;
        
        private System.DateTime NeedStartDateField;
        
        private int NumberField;
        
        private string StateField;
        
        private bool canChangeField;
        
        private bool canCheckField;
        
        private bool changeField;
        
        private bool changedField;
        
        private bool checkField;
        
        private bool initField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime AllowDateEnd {
            get {
                return this.AllowDateEndField;
            }
            set {
                if ((this.AllowDateEndField.Equals(value) != true)) {
                    this.AllowDateEndField = value;
                    this.RaisePropertyChanged("AllowDateEnd");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime AllowDateStart {
            get {
                return this.AllowDateStartField;
            }
            set {
                if ((this.AllowDateStartField.Equals(value) != true)) {
                    this.AllowDateStartField = value;
                    this.RaisePropertyChanged("AllowDateStart");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Allowed {
            get {
                return this.AllowedField;
            }
            set {
                if ((this.AllowedField.Equals(value) != true)) {
                    this.AllowedField = value;
                    this.RaisePropertyChanged("Allowed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Author {
            get {
                return this.AuthorField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthorField, value) != true)) {
                    this.AuthorField = value;
                    this.RaisePropertyChanged("Author");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AuthorAllow {
            get {
                return this.AuthorAllowField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthorAllowField, value) != true)) {
                    this.AuthorAllowField = value;
                    this.RaisePropertyChanged("AuthorAllow");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Comment {
            get {
                return this.CommentField;
            }
            set {
                if ((object.ReferenceEquals(this.CommentField, value) != true)) {
                    this.CommentField = value;
                    this.RaisePropertyChanged("Comment");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CranNumber {
            get {
                return this.CranNumberField;
            }
            set {
                if ((this.CranNumberField.Equals(value) != true)) {
                    this.CranNumberField = value;
                    this.RaisePropertyChanged("CranNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Denied {
            get {
                return this.DeniedField;
            }
            set {
                if ((this.DeniedField.Equals(value) != true)) {
                    this.DeniedField = value;
                    this.RaisePropertyChanged("Denied");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime NeedEndDate {
            get {
                return this.NeedEndDateField;
            }
            set {
                if ((this.NeedEndDateField.Equals(value) != true)) {
                    this.NeedEndDateField = value;
                    this.RaisePropertyChanged("NeedEndDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime NeedStartDate {
            get {
                return this.NeedStartDateField;
            }
            set {
                if ((this.NeedStartDateField.Equals(value) != true)) {
                    this.NeedStartDateField = value;
                    this.RaisePropertyChanged("NeedStartDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Number {
            get {
                return this.NumberField;
            }
            set {
                if ((this.NumberField.Equals(value) != true)) {
                    this.NumberField = value;
                    this.RaisePropertyChanged("Number");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string State {
            get {
                return this.StateField;
            }
            set {
                if ((object.ReferenceEquals(this.StateField, value) != true)) {
                    this.StateField = value;
                    this.RaisePropertyChanged("State");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool canChange {
            get {
                return this.canChangeField;
            }
            set {
                if ((this.canChangeField.Equals(value) != true)) {
                    this.canChangeField = value;
                    this.RaisePropertyChanged("canChange");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool canCheck {
            get {
                return this.canCheckField;
            }
            set {
                if ((this.canCheckField.Equals(value) != true)) {
                    this.canCheckField = value;
                    this.RaisePropertyChanged("canCheck");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool change {
            get {
                return this.changeField;
            }
            set {
                if ((this.changeField.Equals(value) != true)) {
                    this.changeField = value;
                    this.RaisePropertyChanged("change");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool changed {
            get {
                return this.changedField;
            }
            set {
                if ((this.changedField.Equals(value) != true)) {
                    this.changedField = value;
                    this.RaisePropertyChanged("changed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool check {
            get {
                return this.checkField;
            }
            set {
                if ((this.checkField.Equals(value) != true)) {
                    this.checkField = value;
                    this.RaisePropertyChanged("check");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool init {
            get {
                return this.initField;
            }
            set {
                if ((this.initField.Equals(value) != true)) {
                    this.initField = value;
                    this.RaisePropertyChanged("init");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ReturnMessage", Namespace="http://schemas.datacontract.org/2004/07/VotGESOrders.Web.Models")]
    public partial class ReturnMessage : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string MessageField;
        
        private bool ResultField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Result {
            get {
                return this.ResultField;
            }
            set {
                if ((this.ResultField.Equals(value) != true)) {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="CranService.CranService")]
    public interface CranService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:CranService/DoWork", ReplyAction="urn:CranService/DoWorkResponse")]
        System.IAsyncResult BeginDoWork(System.AsyncCallback callback, object asyncState);
        
        void EndDoWork(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:CranService/getCranTasks", ReplyAction="urn:CranService/getCranTasksResponse")]
        System.IAsyncResult BegingetCranTasks(VotGESOrders.CranService.CranFilter Filter, System.AsyncCallback callback, object asyncState);
        
        VotGESOrders.CranService.CranFilter EndgetCranTasks(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:CranService/CreateCranTask", ReplyAction="urn:CranService/CreateCranTaskResponse")]
        System.IAsyncResult BeginCreateCranTask(VotGESOrders.CranService.CranTaskInfo task, System.AsyncCallback callback, object asyncState);
        
        VotGESOrders.CranService.ReturnMessage EndCreateCranTask(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CranServiceChannel : VotGESOrders.CranService.CranService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class getCranTasksCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public getCranTasksCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public VotGESOrders.CranService.CranFilter Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((VotGESOrders.CranService.CranFilter)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CreateCranTaskCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public CreateCranTaskCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public VotGESOrders.CranService.ReturnMessage Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((VotGESOrders.CranService.ReturnMessage)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CranServiceClient : System.ServiceModel.ClientBase<VotGESOrders.CranService.CranService>, VotGESOrders.CranService.CranService {
        
        private BeginOperationDelegate onBeginDoWorkDelegate;
        
        private EndOperationDelegate onEndDoWorkDelegate;
        
        private System.Threading.SendOrPostCallback onDoWorkCompletedDelegate;
        
        private BeginOperationDelegate onBegingetCranTasksDelegate;
        
        private EndOperationDelegate onEndgetCranTasksDelegate;
        
        private System.Threading.SendOrPostCallback ongetCranTasksCompletedDelegate;
        
        private BeginOperationDelegate onBeginCreateCranTaskDelegate;
        
        private EndOperationDelegate onEndCreateCranTaskDelegate;
        
        private System.Threading.SendOrPostCallback onCreateCranTaskCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public CranServiceClient() {
        }
        
        public CranServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CranServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CranServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CranServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Не удалось установить CookieContainer. Убедитесь, что привязка содержит HttpCooki" +
                            "eContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DoWorkCompleted;
        
        public event System.EventHandler<getCranTasksCompletedEventArgs> getCranTasksCompleted;
        
        public event System.EventHandler<CreateCranTaskCompletedEventArgs> CreateCranTaskCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult VotGESOrders.CranService.CranService.BeginDoWork(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDoWork(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void VotGESOrders.CranService.CranService.EndDoWork(System.IAsyncResult result) {
            base.Channel.EndDoWork(result);
        }
        
        private System.IAsyncResult OnBeginDoWork(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((VotGESOrders.CranService.CranService)(this)).BeginDoWork(callback, asyncState);
        }
        
        private object[] OnEndDoWork(System.IAsyncResult result) {
            ((VotGESOrders.CranService.CranService)(this)).EndDoWork(result);
            return null;
        }
        
        private void OnDoWorkCompleted(object state) {
            if ((this.DoWorkCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DoWorkCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DoWorkAsync() {
            this.DoWorkAsync(null);
        }
        
        public void DoWorkAsync(object userState) {
            if ((this.onBeginDoWorkDelegate == null)) {
                this.onBeginDoWorkDelegate = new BeginOperationDelegate(this.OnBeginDoWork);
            }
            if ((this.onEndDoWorkDelegate == null)) {
                this.onEndDoWorkDelegate = new EndOperationDelegate(this.OnEndDoWork);
            }
            if ((this.onDoWorkCompletedDelegate == null)) {
                this.onDoWorkCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDoWorkCompleted);
            }
            base.InvokeAsync(this.onBeginDoWorkDelegate, null, this.onEndDoWorkDelegate, this.onDoWorkCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult VotGESOrders.CranService.CranService.BegingetCranTasks(VotGESOrders.CranService.CranFilter Filter, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BegingetCranTasks(Filter, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        VotGESOrders.CranService.CranFilter VotGESOrders.CranService.CranService.EndgetCranTasks(System.IAsyncResult result) {
            return base.Channel.EndgetCranTasks(result);
        }
        
        private System.IAsyncResult OnBegingetCranTasks(object[] inValues, System.AsyncCallback callback, object asyncState) {
            VotGESOrders.CranService.CranFilter Filter = ((VotGESOrders.CranService.CranFilter)(inValues[0]));
            return ((VotGESOrders.CranService.CranService)(this)).BegingetCranTasks(Filter, callback, asyncState);
        }
        
        private object[] OnEndgetCranTasks(System.IAsyncResult result) {
            VotGESOrders.CranService.CranFilter retVal = ((VotGESOrders.CranService.CranService)(this)).EndgetCranTasks(result);
            return new object[] {
                    retVal};
        }
        
        private void OngetCranTasksCompleted(object state) {
            if ((this.getCranTasksCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.getCranTasksCompleted(this, new getCranTasksCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void getCranTasksAsync(VotGESOrders.CranService.CranFilter Filter) {
            this.getCranTasksAsync(Filter, null);
        }
        
        public void getCranTasksAsync(VotGESOrders.CranService.CranFilter Filter, object userState) {
            if ((this.onBegingetCranTasksDelegate == null)) {
                this.onBegingetCranTasksDelegate = new BeginOperationDelegate(this.OnBegingetCranTasks);
            }
            if ((this.onEndgetCranTasksDelegate == null)) {
                this.onEndgetCranTasksDelegate = new EndOperationDelegate(this.OnEndgetCranTasks);
            }
            if ((this.ongetCranTasksCompletedDelegate == null)) {
                this.ongetCranTasksCompletedDelegate = new System.Threading.SendOrPostCallback(this.OngetCranTasksCompleted);
            }
            base.InvokeAsync(this.onBegingetCranTasksDelegate, new object[] {
                        Filter}, this.onEndgetCranTasksDelegate, this.ongetCranTasksCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult VotGESOrders.CranService.CranService.BeginCreateCranTask(VotGESOrders.CranService.CranTaskInfo task, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginCreateCranTask(task, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        VotGESOrders.CranService.ReturnMessage VotGESOrders.CranService.CranService.EndCreateCranTask(System.IAsyncResult result) {
            return base.Channel.EndCreateCranTask(result);
        }
        
        private System.IAsyncResult OnBeginCreateCranTask(object[] inValues, System.AsyncCallback callback, object asyncState) {
            VotGESOrders.CranService.CranTaskInfo task = ((VotGESOrders.CranService.CranTaskInfo)(inValues[0]));
            return ((VotGESOrders.CranService.CranService)(this)).BeginCreateCranTask(task, callback, asyncState);
        }
        
        private object[] OnEndCreateCranTask(System.IAsyncResult result) {
            VotGESOrders.CranService.ReturnMessage retVal = ((VotGESOrders.CranService.CranService)(this)).EndCreateCranTask(result);
            return new object[] {
                    retVal};
        }
        
        private void OnCreateCranTaskCompleted(object state) {
            if ((this.CreateCranTaskCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CreateCranTaskCompleted(this, new CreateCranTaskCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CreateCranTaskAsync(VotGESOrders.CranService.CranTaskInfo task) {
            this.CreateCranTaskAsync(task, null);
        }
        
        public void CreateCranTaskAsync(VotGESOrders.CranService.CranTaskInfo task, object userState) {
            if ((this.onBeginCreateCranTaskDelegate == null)) {
                this.onBeginCreateCranTaskDelegate = new BeginOperationDelegate(this.OnBeginCreateCranTask);
            }
            if ((this.onEndCreateCranTaskDelegate == null)) {
                this.onEndCreateCranTaskDelegate = new EndOperationDelegate(this.OnEndCreateCranTask);
            }
            if ((this.onCreateCranTaskCompletedDelegate == null)) {
                this.onCreateCranTaskCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCreateCranTaskCompleted);
            }
            base.InvokeAsync(this.onBeginCreateCranTaskDelegate, new object[] {
                        task}, this.onEndCreateCranTaskDelegate, this.onCreateCranTaskCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override VotGESOrders.CranService.CranService CreateChannel() {
            return new CranServiceClientChannel(this);
        }
        
        private class CranServiceClientChannel : ChannelBase<VotGESOrders.CranService.CranService>, VotGESOrders.CranService.CranService {
            
            public CranServiceClientChannel(System.ServiceModel.ClientBase<VotGESOrders.CranService.CranService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginDoWork(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("DoWork", _args, callback, asyncState);
                return _result;
            }
            
            public void EndDoWork(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("DoWork", _args, result);
            }
            
            public System.IAsyncResult BegingetCranTasks(VotGESOrders.CranService.CranFilter Filter, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = Filter;
                System.IAsyncResult _result = base.BeginInvoke("getCranTasks", _args, callback, asyncState);
                return _result;
            }
            
            public VotGESOrders.CranService.CranFilter EndgetCranTasks(System.IAsyncResult result) {
                object[] _args = new object[0];
                VotGESOrders.CranService.CranFilter _result = ((VotGESOrders.CranService.CranFilter)(base.EndInvoke("getCranTasks", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginCreateCranTask(VotGESOrders.CranService.CranTaskInfo task, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = task;
                System.IAsyncResult _result = base.BeginInvoke("CreateCranTask", _args, callback, asyncState);
                return _result;
            }
            
            public VotGESOrders.CranService.ReturnMessage EndCreateCranTask(System.IAsyncResult result) {
                object[] _args = new object[0];
                VotGESOrders.CranService.ReturnMessage _result = ((VotGESOrders.CranService.ReturnMessage)(base.EndInvoke("CreateCranTask", _args, result)));
                return _result;
            }
        }
    }
}
