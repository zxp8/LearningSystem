namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：QuesKnowledge 主键列：Qk_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class QuesKnowledge : WeiSha.Data.Entity {
    		
    		protected Int64 _Qk_ID;
    		
    		protected Int32 _Org_ID;
    		
    		protected Int32 _Qk_Count;
    		
    		protected DateTime _Qk_CrtTime;
    		
    		protected DateTime _Qk_DeleteTime;
    		
    		protected String _Qk_Details;
    		
    		protected String _Qk_Intro;
    		
    		protected Boolean _Qk_IsDeleted;
    		
    		protected Boolean _Qk_IsUse;
    		
    		protected String _Qk_Name;
    		
    		protected Int32 _Qk_Order;
    		
    		protected Int64 _Qk_PID;
    		
    		protected DateTime _Qk_UpdateTime;
    		
    		protected Int32 _Qk_Weight;
    		
    		public Int64 Qk_ID {
    			get {
    				return this._Qk_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_ID, _Qk_ID, value);
    				this._Qk_ID = value;
    			}
    		}
    		
    		public Int32 Org_ID {
    			get {
    				return this._Org_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Org_ID, _Org_ID, value);
    				this._Org_ID = value;
    			}
    		}
    		
    		public Int32 Qk_Count {
    			get {
    				return this._Qk_Count;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Count, _Qk_Count, value);
    				this._Qk_Count = value;
    			}
    		}
    		
    		public DateTime Qk_CrtTime {
    			get {
    				return this._Qk_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_CrtTime, _Qk_CrtTime, value);
    				this._Qk_CrtTime = value;
    			}
    		}
    		
    		public DateTime Qk_DeleteTime {
    			get {
    				return this._Qk_DeleteTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_DeleteTime, _Qk_DeleteTime, value);
    				this._Qk_DeleteTime = value;
    			}
    		}
    		
    		public String Qk_Details {
    			get {
    				return this._Qk_Details;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Details, _Qk_Details, value);
    				this._Qk_Details = value;
    			}
    		}
    		
    		public String Qk_Intro {
    			get {
    				return this._Qk_Intro;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Intro, _Qk_Intro, value);
    				this._Qk_Intro = value;
    			}
    		}
    		
    		public Boolean Qk_IsDeleted {
    			get {
    				return this._Qk_IsDeleted;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_IsDeleted, _Qk_IsDeleted, value);
    				this._Qk_IsDeleted = value;
    			}
    		}
    		
    		public Boolean Qk_IsUse {
    			get {
    				return this._Qk_IsUse;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_IsUse, _Qk_IsUse, value);
    				this._Qk_IsUse = value;
    			}
    		}
    		
    		public String Qk_Name {
    			get {
    				return this._Qk_Name;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Name, _Qk_Name, value);
    				this._Qk_Name = value;
    			}
    		}
    		
    		public Int32 Qk_Order {
    			get {
    				return this._Qk_Order;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Order, _Qk_Order, value);
    				this._Qk_Order = value;
    			}
    		}
    		
    		public Int64 Qk_PID {
    			get {
    				return this._Qk_PID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_PID, _Qk_PID, value);
    				this._Qk_PID = value;
    			}
    		}
    		
    		public DateTime Qk_UpdateTime {
    			get {
    				return this._Qk_UpdateTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_UpdateTime, _Qk_UpdateTime, value);
    				this._Qk_UpdateTime = value;
    			}
    		}
    		
    		public Int32 Qk_Weight {
    			get {
    				return this._Qk_Weight;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_Weight, _Qk_Weight, value);
    				this._Qk_Weight = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<QuesKnowledge>("QuesKnowledge");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qk_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qk_ID,
    					_.Org_ID,
    					_.Qk_Count,
    					_.Qk_CrtTime,
    					_.Qk_DeleteTime,
    					_.Qk_Details,
    					_.Qk_Intro,
    					_.Qk_IsDeleted,
    					_.Qk_IsUse,
    					_.Qk_Name,
    					_.Qk_Order,
    					_.Qk_PID,
    					_.Qk_UpdateTime,
    					_.Qk_Weight};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qk_ID,
    					this._Org_ID,
    					this._Qk_Count,
    					this._Qk_CrtTime,
    					this._Qk_DeleteTime,
    					this._Qk_Details,
    					this._Qk_Intro,
    					this._Qk_IsDeleted,
    					this._Qk_IsUse,
    					this._Qk_Name,
    					this._Qk_Order,
    					this._Qk_PID,
    					this._Qk_UpdateTime,
    					this._Qk_Weight};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qk_ID))) {
    				this._Qk_ID = reader.GetInt64(_.Qk_ID);
    			}
    			if ((false == reader.IsDBNull(_.Org_ID))) {
    				this._Org_ID = reader.GetInt32(_.Org_ID);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Count))) {
    				this._Qk_Count = reader.GetInt32(_.Qk_Count);
    			}
    			if ((false == reader.IsDBNull(_.Qk_CrtTime))) {
    				this._Qk_CrtTime = reader.GetDateTime(_.Qk_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Qk_DeleteTime))) {
    				this._Qk_DeleteTime = reader.GetDateTime(_.Qk_DeleteTime);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Details))) {
    				this._Qk_Details = reader.GetString(_.Qk_Details);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Intro))) {
    				this._Qk_Intro = reader.GetString(_.Qk_Intro);
    			}
    			if ((false == reader.IsDBNull(_.Qk_IsDeleted))) {
    				this._Qk_IsDeleted = reader.GetBoolean(_.Qk_IsDeleted);
    			}
    			if ((false == reader.IsDBNull(_.Qk_IsUse))) {
    				this._Qk_IsUse = reader.GetBoolean(_.Qk_IsUse);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Name))) {
    				this._Qk_Name = reader.GetString(_.Qk_Name);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Order))) {
    				this._Qk_Order = reader.GetInt32(_.Qk_Order);
    			}
    			if ((false == reader.IsDBNull(_.Qk_PID))) {
    				this._Qk_PID = reader.GetInt64(_.Qk_PID);
    			}
    			if ((false == reader.IsDBNull(_.Qk_UpdateTime))) {
    				this._Qk_UpdateTime = reader.GetDateTime(_.Qk_UpdateTime);
    			}
    			if ((false == reader.IsDBNull(_.Qk_Weight))) {
    				this._Qk_Weight = reader.GetInt32(_.Qk_Weight);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(QuesKnowledge).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<QuesKnowledge>();
    			
    			/// <summary>
    			/// 字段名：Qk_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qk_ID = new WeiSha.Data.Field<QuesKnowledge>("Qk_ID");
    			
    			/// <summary>
    			/// 字段名：Org_ID - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Org_ID = new WeiSha.Data.Field<QuesKnowledge>("Org_ID");
    			
    			/// <summary>
    			/// 字段名：Qk_Count - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Count = new WeiSha.Data.Field<QuesKnowledge>("Qk_Count");
    			
    			/// <summary>
    			/// 字段名：Qk_CrtTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qk_CrtTime = new WeiSha.Data.Field<QuesKnowledge>("Qk_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Qk_DeleteTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qk_DeleteTime = new WeiSha.Data.Field<QuesKnowledge>("Qk_DeleteTime");
    			
    			/// <summary>
    			/// 字段名：Qk_Details - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Details = new WeiSha.Data.Field<QuesKnowledge>("Qk_Details");
    			
    			/// <summary>
    			/// 字段名：Qk_Intro - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Intro = new WeiSha.Data.Field<QuesKnowledge>("Qk_Intro");
    			
    			/// <summary>
    			/// 字段名：Qk_IsDeleted - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Qk_IsDeleted = new WeiSha.Data.Field<QuesKnowledge>("Qk_IsDeleted");
    			
    			/// <summary>
    			/// 字段名：Qk_IsUse - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Qk_IsUse = new WeiSha.Data.Field<QuesKnowledge>("Qk_IsUse");
    			
    			/// <summary>
    			/// 字段名：Qk_Name - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Name = new WeiSha.Data.Field<QuesKnowledge>("Qk_Name");
    			
    			/// <summary>
    			/// 字段名：Qk_Order - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Order = new WeiSha.Data.Field<QuesKnowledge>("Qk_Order");
    			
    			/// <summary>
    			/// 字段名：Qk_PID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qk_PID = new WeiSha.Data.Field<QuesKnowledge>("Qk_PID");
    			
    			/// <summary>
    			/// 字段名：Qk_UpdateTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qk_UpdateTime = new WeiSha.Data.Field<QuesKnowledge>("Qk_UpdateTime");
    			
    			/// <summary>
    			/// 字段名：Qk_Weight - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qk_Weight = new WeiSha.Data.Field<QuesKnowledge>("Qk_Weight");
    		}
    	}
    }
    