namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：QuesTags 主键列：Qtag_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class QuesTags : WeiSha.Data.Entity {
    		
    		protected Int64 _Qtag_ID;
    		
    		protected Int64 _Cou_ID;
    		
    		protected Int32 _Org_ID;
    		
    		protected Int32 _Qtag_Count;
    		
    		protected DateTime _Qtag_CrtTime;
    		
    		protected DateTime _Qtag_DeleteTime;
    		
    		protected Boolean _Qtag_IsDeleted;
    		
    		protected String _Qtag_Name;
    		
    		protected Int32 _Qtag_Order;
    		
    		protected Int64 _Qtag_PID;
    		
    		protected DateTime _Qtag_UpdateTime;
    		
    		protected Int32 _Qtag_Weight;
    		
    		public Int64 Qtag_ID {
    			get {
    				return this._Qtag_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_ID, _Qtag_ID, value);
    				this._Qtag_ID = value;
    			}
    		}
    		
    		public Int64 Cou_ID {
    			get {
    				return this._Cou_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Cou_ID, _Cou_ID, value);
    				this._Cou_ID = value;
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
    		
    		public Int32 Qtag_Count {
    			get {
    				return this._Qtag_Count;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_Count, _Qtag_Count, value);
    				this._Qtag_Count = value;
    			}
    		}
    		
    		public DateTime Qtag_CrtTime {
    			get {
    				return this._Qtag_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_CrtTime, _Qtag_CrtTime, value);
    				this._Qtag_CrtTime = value;
    			}
    		}
    		
    		public DateTime Qtag_DeleteTime {
    			get {
    				return this._Qtag_DeleteTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_DeleteTime, _Qtag_DeleteTime, value);
    				this._Qtag_DeleteTime = value;
    			}
    		}
    		
    		public Boolean Qtag_IsDeleted {
    			get {
    				return this._Qtag_IsDeleted;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_IsDeleted, _Qtag_IsDeleted, value);
    				this._Qtag_IsDeleted = value;
    			}
    		}
    		
    		public String Qtag_Name {
    			get {
    				return this._Qtag_Name;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_Name, _Qtag_Name, value);
    				this._Qtag_Name = value;
    			}
    		}
    		
    		public Int32 Qtag_Order {
    			get {
    				return this._Qtag_Order;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_Order, _Qtag_Order, value);
    				this._Qtag_Order = value;
    			}
    		}
    		
    		public Int64 Qtag_PID {
    			get {
    				return this._Qtag_PID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_PID, _Qtag_PID, value);
    				this._Qtag_PID = value;
    			}
    		}
    		
    		public DateTime Qtag_UpdateTime {
    			get {
    				return this._Qtag_UpdateTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_UpdateTime, _Qtag_UpdateTime, value);
    				this._Qtag_UpdateTime = value;
    			}
    		}
    		
    		public Int32 Qtag_Weight {
    			get {
    				return this._Qtag_Weight;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qtag_Weight, _Qtag_Weight, value);
    				this._Qtag_Weight = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<QuesTags>("QuesTags");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qtag_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qtag_ID,
    					_.Cou_ID,
    					_.Org_ID,
    					_.Qtag_Count,
    					_.Qtag_CrtTime,
    					_.Qtag_DeleteTime,
    					_.Qtag_IsDeleted,
    					_.Qtag_Name,
    					_.Qtag_Order,
    					_.Qtag_PID,
    					_.Qtag_UpdateTime,
    					_.Qtag_Weight};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qtag_ID,
    					this._Cou_ID,
    					this._Org_ID,
    					this._Qtag_Count,
    					this._Qtag_CrtTime,
    					this._Qtag_DeleteTime,
    					this._Qtag_IsDeleted,
    					this._Qtag_Name,
    					this._Qtag_Order,
    					this._Qtag_PID,
    					this._Qtag_UpdateTime,
    					this._Qtag_Weight};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qtag_ID))) {
    				this._Qtag_ID = reader.GetInt64(_.Qtag_ID);
    			}
    			if ((false == reader.IsDBNull(_.Cou_ID))) {
    				this._Cou_ID = reader.GetInt64(_.Cou_ID);
    			}
    			if ((false == reader.IsDBNull(_.Org_ID))) {
    				this._Org_ID = reader.GetInt32(_.Org_ID);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_Count))) {
    				this._Qtag_Count = reader.GetInt32(_.Qtag_Count);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_CrtTime))) {
    				this._Qtag_CrtTime = reader.GetDateTime(_.Qtag_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_DeleteTime))) {
    				this._Qtag_DeleteTime = reader.GetDateTime(_.Qtag_DeleteTime);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_IsDeleted))) {
    				this._Qtag_IsDeleted = reader.GetBoolean(_.Qtag_IsDeleted);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_Name))) {
    				this._Qtag_Name = reader.GetString(_.Qtag_Name);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_Order))) {
    				this._Qtag_Order = reader.GetInt32(_.Qtag_Order);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_PID))) {
    				this._Qtag_PID = reader.GetInt64(_.Qtag_PID);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_UpdateTime))) {
    				this._Qtag_UpdateTime = reader.GetDateTime(_.Qtag_UpdateTime);
    			}
    			if ((false == reader.IsDBNull(_.Qtag_Weight))) {
    				this._Qtag_Weight = reader.GetInt32(_.Qtag_Weight);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(QuesTags).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<QuesTags>();
    			
    			/// <summary>
    			/// 字段名：Qtag_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_ID = new WeiSha.Data.Field<QuesTags>("Qtag_ID");
    			
    			/// <summary>
    			/// 字段名：Cou_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Cou_ID = new WeiSha.Data.Field<QuesTags>("Cou_ID");
    			
    			/// <summary>
    			/// 字段名：Org_ID - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Org_ID = new WeiSha.Data.Field<QuesTags>("Org_ID");
    			
    			/// <summary>
    			/// 字段名：Qtag_Count - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_Count = new WeiSha.Data.Field<QuesTags>("Qtag_Count");
    			
    			/// <summary>
    			/// 字段名：Qtag_CrtTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_CrtTime = new WeiSha.Data.Field<QuesTags>("Qtag_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Qtag_DeleteTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_DeleteTime = new WeiSha.Data.Field<QuesTags>("Qtag_DeleteTime");
    			
    			/// <summary>
    			/// 字段名：Qtag_IsDeleted - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_IsDeleted = new WeiSha.Data.Field<QuesTags>("Qtag_IsDeleted");
    			
    			/// <summary>
    			/// 字段名：Qtag_Name - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_Name = new WeiSha.Data.Field<QuesTags>("Qtag_Name");
    			
    			/// <summary>
    			/// 字段名：Qtag_Order - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_Order = new WeiSha.Data.Field<QuesTags>("Qtag_Order");
    			
    			/// <summary>
    			/// 字段名：Qtag_PID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_PID = new WeiSha.Data.Field<QuesTags>("Qtag_PID");
    			
    			/// <summary>
    			/// 字段名：Qtag_UpdateTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_UpdateTime = new WeiSha.Data.Field<QuesTags>("Qtag_UpdateTime");
    			
    			/// <summary>
    			/// 字段名：Qtag_Weight - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qtag_Weight = new WeiSha.Data.Field<QuesTags>("Qtag_Weight");
    		}
    	}
    }
    