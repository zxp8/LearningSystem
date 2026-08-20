namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：DataOperateLog 主键列：Dlog_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class DataOperateLog : WeiSha.Data.Entity {
    		
    		protected Int64 _Dlog_ID;
    		
    		protected Int64? _Ac_ID;
    		
    		protected Int64? _Acc_ID;
    		
    		protected String _Dlog_API;
    		
    		protected String _Dlog_Browser;
    		
    		protected String _Dlog_BrwUa;
    		
    		protected DateTime _Dlog_CrtTime;
    		
    		protected String _Dlog_Entity;
    		
    		protected String _Dlog_Fields;
    		
    		protected String _Dlog_IP;
    		
    		protected Int64 _Dlog_KeyID;
    		
    		protected String _Dlog_Mark;
    		
    		protected String _Dlog_Module;
    		
    		protected String _Dlog_NewData;
    		
    		protected String _Dlog_OS;
    		
    		protected String _Dlog_OldData;
    		
    		protected Int32? _Dlog_Timespan;
    		
    		protected Int16 _Dlog_Type;
    		
    		protected String _MM_Link;
    		
    		protected String _MM_UID;
    		
    		protected Int64? _Org_ID;
    		
    		protected Int64? _Th_ID;
    		
    		public Int64 Dlog_ID {
    			get {
    				return this._Dlog_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_ID, _Dlog_ID, value);
    				this._Dlog_ID = value;
    			}
    		}
    		
    		public Int64? Ac_ID {
    			get {
    				return this._Ac_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Ac_ID, _Ac_ID, value);
    				this._Ac_ID = value;
    			}
    		}
    		
    		public Int64? Acc_ID {
    			get {
    				return this._Acc_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Acc_ID, _Acc_ID, value);
    				this._Acc_ID = value;
    			}
    		}
    		
    		public String Dlog_API {
    			get {
    				return this._Dlog_API;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_API, _Dlog_API, value);
    				this._Dlog_API = value;
    			}
    		}
    		
    		public String Dlog_Browser {
    			get {
    				return this._Dlog_Browser;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Browser, _Dlog_Browser, value);
    				this._Dlog_Browser = value;
    			}
    		}
    		
    		public String Dlog_BrwUa {
    			get {
    				return this._Dlog_BrwUa;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_BrwUa, _Dlog_BrwUa, value);
    				this._Dlog_BrwUa = value;
    			}
    		}
    		
    		public DateTime Dlog_CrtTime {
    			get {
    				return this._Dlog_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_CrtTime, _Dlog_CrtTime, value);
    				this._Dlog_CrtTime = value;
    			}
    		}
    		
    		public String Dlog_Entity {
    			get {
    				return this._Dlog_Entity;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Entity, _Dlog_Entity, value);
    				this._Dlog_Entity = value;
    			}
    		}
    		
    		public String Dlog_Fields {
    			get {
    				return this._Dlog_Fields;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Fields, _Dlog_Fields, value);
    				this._Dlog_Fields = value;
    			}
    		}
    		
    		public String Dlog_IP {
    			get {
    				return this._Dlog_IP;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_IP, _Dlog_IP, value);
    				this._Dlog_IP = value;
    			}
    		}
    		
    		public Int64 Dlog_KeyID {
    			get {
    				return this._Dlog_KeyID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_KeyID, _Dlog_KeyID, value);
    				this._Dlog_KeyID = value;
    			}
    		}
    		
    		public String Dlog_Mark {
    			get {
    				return this._Dlog_Mark;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Mark, _Dlog_Mark, value);
    				this._Dlog_Mark = value;
    			}
    		}
    		
    		public String Dlog_Module {
    			get {
    				return this._Dlog_Module;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Module, _Dlog_Module, value);
    				this._Dlog_Module = value;
    			}
    		}
    		
    		public String Dlog_NewData {
    			get {
    				return this._Dlog_NewData;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_NewData, _Dlog_NewData, value);
    				this._Dlog_NewData = value;
    			}
    		}
    		
    		public String Dlog_OS {
    			get {
    				return this._Dlog_OS;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_OS, _Dlog_OS, value);
    				this._Dlog_OS = value;
    			}
    		}
    		
    		public String Dlog_OldData {
    			get {
    				return this._Dlog_OldData;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_OldData, _Dlog_OldData, value);
    				this._Dlog_OldData = value;
    			}
    		}
    		
    		public Int32? Dlog_Timespan {
    			get {
    				return this._Dlog_Timespan;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Timespan, _Dlog_Timespan, value);
    				this._Dlog_Timespan = value;
    			}
    		}
    		
    		public Int16 Dlog_Type {
    			get {
    				return this._Dlog_Type;
    			}
    			set {
    				this.OnPropertyValueChange(_.Dlog_Type, _Dlog_Type, value);
    				this._Dlog_Type = value;
    			}
    		}
    		
    		public String MM_Link {
    			get {
    				return this._MM_Link;
    			}
    			set {
    				this.OnPropertyValueChange(_.MM_Link, _MM_Link, value);
    				this._MM_Link = value;
    			}
    		}
    		
    		public String MM_UID {
    			get {
    				return this._MM_UID;
    			}
    			set {
    				this.OnPropertyValueChange(_.MM_UID, _MM_UID, value);
    				this._MM_UID = value;
    			}
    		}
    		
    		public Int64? Org_ID {
    			get {
    				return this._Org_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Org_ID, _Org_ID, value);
    				this._Org_ID = value;
    			}
    		}
    		
    		public Int64? Th_ID {
    			get {
    				return this._Th_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Th_ID, _Th_ID, value);
    				this._Th_ID = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<DataOperateLog>("DataOperateLog");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Dlog_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Dlog_ID,
    					_.Ac_ID,
    					_.Acc_ID,
    					_.Dlog_API,
    					_.Dlog_Browser,
    					_.Dlog_BrwUa,
    					_.Dlog_CrtTime,
    					_.Dlog_Entity,
    					_.Dlog_Fields,
    					_.Dlog_IP,
    					_.Dlog_KeyID,
    					_.Dlog_Mark,
    					_.Dlog_Module,
    					_.Dlog_NewData,
    					_.Dlog_OS,
    					_.Dlog_OldData,
    					_.Dlog_Timespan,
    					_.Dlog_Type,
    					_.MM_Link,
    					_.MM_UID,
    					_.Org_ID,
    					_.Th_ID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Dlog_ID,
    					this._Ac_ID,
    					this._Acc_ID,
    					this._Dlog_API,
    					this._Dlog_Browser,
    					this._Dlog_BrwUa,
    					this._Dlog_CrtTime,
    					this._Dlog_Entity,
    					this._Dlog_Fields,
    					this._Dlog_IP,
    					this._Dlog_KeyID,
    					this._Dlog_Mark,
    					this._Dlog_Module,
    					this._Dlog_NewData,
    					this._Dlog_OS,
    					this._Dlog_OldData,
    					this._Dlog_Timespan,
    					this._Dlog_Type,
    					this._MM_Link,
    					this._MM_UID,
    					this._Org_ID,
    					this._Th_ID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Dlog_ID))) {
    				this._Dlog_ID = reader.GetInt64(_.Dlog_ID);
    			}
    			if ((false == reader.IsDBNull(_.Ac_ID))) {
    				this._Ac_ID = reader.GetInt64(_.Ac_ID);
    			}
    			if ((false == reader.IsDBNull(_.Acc_ID))) {
    				this._Acc_ID = reader.GetInt64(_.Acc_ID);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_API))) {
    				this._Dlog_API = reader.GetString(_.Dlog_API);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Browser))) {
    				this._Dlog_Browser = reader.GetString(_.Dlog_Browser);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_BrwUa))) {
    				this._Dlog_BrwUa = reader.GetString(_.Dlog_BrwUa);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_CrtTime))) {
    				this._Dlog_CrtTime = reader.GetDateTime(_.Dlog_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Entity))) {
    				this._Dlog_Entity = reader.GetString(_.Dlog_Entity);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Fields))) {
    				this._Dlog_Fields = reader.GetString(_.Dlog_Fields);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_IP))) {
    				this._Dlog_IP = reader.GetString(_.Dlog_IP);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_KeyID))) {
    				this._Dlog_KeyID = reader.GetInt64(_.Dlog_KeyID);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Mark))) {
    				this._Dlog_Mark = reader.GetString(_.Dlog_Mark);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Module))) {
    				this._Dlog_Module = reader.GetString(_.Dlog_Module);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_NewData))) {
    				this._Dlog_NewData = reader.GetString(_.Dlog_NewData);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_OS))) {
    				this._Dlog_OS = reader.GetString(_.Dlog_OS);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_OldData))) {
    				this._Dlog_OldData = reader.GetString(_.Dlog_OldData);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Timespan))) {
    				this._Dlog_Timespan = reader.GetInt32(_.Dlog_Timespan);
    			}
    			if ((false == reader.IsDBNull(_.Dlog_Type))) {
    				this._Dlog_Type = reader.GetInt16(_.Dlog_Type);
    			}
    			if ((false == reader.IsDBNull(_.MM_Link))) {
    				this._MM_Link = reader.GetString(_.MM_Link);
    			}
    			if ((false == reader.IsDBNull(_.MM_UID))) {
    				this._MM_UID = reader.GetString(_.MM_UID);
    			}
    			if ((false == reader.IsDBNull(_.Org_ID))) {
    				this._Org_ID = reader.GetInt64(_.Org_ID);
    			}
    			if ((false == reader.IsDBNull(_.Th_ID))) {
    				this._Th_ID = reader.GetInt64(_.Th_ID);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(DataOperateLog).IsAssignableFrom(obj.GetType()))) {
    				return false;
    			}
    			if ((((object)(this)) == ((object)(obj)))) {
    				return true;
    			}
    			return false;
    		}
    		
    		public class _ {
    			
    			/// <summary>
    			/// 表示选择所有列，与*等同
    			/// </summary>
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<DataOperateLog>();
    			
    			/// <summary>
    			/// 字段名：Dlog_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_ID = new WeiSha.Data.Field<DataOperateLog>("Dlog_ID");
    			
    			/// <summary>
    			/// 字段名：Ac_ID - 数据类型：Int64(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Ac_ID = new WeiSha.Data.Field<DataOperateLog>("Ac_ID");
    			
    			/// <summary>
    			/// 字段名：Acc_ID - 数据类型：Int64(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Acc_ID = new WeiSha.Data.Field<DataOperateLog>("Acc_ID");
    			
    			/// <summary>
    			/// 字段名：Dlog_API - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_API = new WeiSha.Data.Field<DataOperateLog>("Dlog_API");
    			
    			/// <summary>
    			/// 字段名：Dlog_Browser - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Browser = new WeiSha.Data.Field<DataOperateLog>("Dlog_Browser");
    			
    			/// <summary>
    			/// 字段名：Dlog_BrwUa - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_BrwUa = new WeiSha.Data.Field<DataOperateLog>("Dlog_BrwUa");
    			
    			/// <summary>
    			/// 字段名：Dlog_CrtTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_CrtTime = new WeiSha.Data.Field<DataOperateLog>("Dlog_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Dlog_Entity - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Entity = new WeiSha.Data.Field<DataOperateLog>("Dlog_Entity");
    			
    			/// <summary>
    			/// 字段名：Dlog_Fields - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Fields = new WeiSha.Data.Field<DataOperateLog>("Dlog_Fields");
    			
    			/// <summary>
    			/// 字段名：Dlog_IP - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_IP = new WeiSha.Data.Field<DataOperateLog>("Dlog_IP");
    			
    			/// <summary>
    			/// 字段名：Dlog_KeyID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_KeyID = new WeiSha.Data.Field<DataOperateLog>("Dlog_KeyID");
    			
    			/// <summary>
    			/// 字段名：Dlog_Mark - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Mark = new WeiSha.Data.Field<DataOperateLog>("Dlog_Mark");
    			
    			/// <summary>
    			/// 字段名：Dlog_Module - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Module = new WeiSha.Data.Field<DataOperateLog>("Dlog_Module");
    			
    			/// <summary>
    			/// 字段名：Dlog_NewData - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_NewData = new WeiSha.Data.Field<DataOperateLog>("Dlog_NewData");
    			
    			/// <summary>
    			/// 字段名：Dlog_OS - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_OS = new WeiSha.Data.Field<DataOperateLog>("Dlog_OS");
    			
    			/// <summary>
    			/// 字段名：Dlog_OldData - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_OldData = new WeiSha.Data.Field<DataOperateLog>("Dlog_OldData");
    			
    			/// <summary>
    			/// 字段名：Dlog_Timespan - 数据类型：Int32(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Timespan = new WeiSha.Data.Field<DataOperateLog>("Dlog_Timespan");
    			
    			/// <summary>
    			/// 字段名：Dlog_Type - 数据类型：Int16
    			/// </summary>
    			public static WeiSha.Data.Field Dlog_Type = new WeiSha.Data.Field<DataOperateLog>("Dlog_Type");
    			
    			/// <summary>
    			/// 字段名：MM_Link - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field MM_Link = new WeiSha.Data.Field<DataOperateLog>("MM_Link");
    			
    			/// <summary>
    			/// 字段名：MM_UID - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field MM_UID = new WeiSha.Data.Field<DataOperateLog>("MM_UID");
    			
    			/// <summary>
    			/// 字段名：Org_ID - 数据类型：Int64(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Org_ID = new WeiSha.Data.Field<DataOperateLog>("Org_ID");
    			
    			/// <summary>
    			/// 字段名：Th_ID - 数据类型：Int64(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Th_ID = new WeiSha.Data.Field<DataOperateLog>("Th_ID");
    		}
    	}
    }
    