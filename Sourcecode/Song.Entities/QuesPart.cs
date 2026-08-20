namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：QuesPart 主键列：Qp_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class QuesPart : WeiSha.Data.Entity {
    		
    		protected Int64 _Qp_ID;
    		
    		protected Int32 _Org_ID;
    		
    		protected Int32 _QP_Count;
    		
    		protected DateTime _Qp_CrtTime;
    		
    		protected DateTime _Qp_DeleteTime;
    		
    		protected String _Qp_Intro;
    		
    		protected Boolean _Qp_IsDeleted;
    		
    		protected Boolean _Qp_IsUse;
    		
    		protected String _Qp_Name;
    		
    		protected Int32 _Qp_Order;
    		
    		protected Int64 _Qp_PID;
    		
    		protected DateTime _Qp_UpdateTime;
    		
    		public Int64 Qp_ID {
    			get {
    				return this._Qp_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_ID, _Qp_ID, value);
    				this._Qp_ID = value;
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
    		
    		public Int32 QP_Count {
    			get {
    				return this._QP_Count;
    			}
    			set {
    				this.OnPropertyValueChange(_.QP_Count, _QP_Count, value);
    				this._QP_Count = value;
    			}
    		}
    		
    		public DateTime Qp_CrtTime {
    			get {
    				return this._Qp_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_CrtTime, _Qp_CrtTime, value);
    				this._Qp_CrtTime = value;
    			}
    		}
    		
    		public DateTime Qp_DeleteTime {
    			get {
    				return this._Qp_DeleteTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_DeleteTime, _Qp_DeleteTime, value);
    				this._Qp_DeleteTime = value;
    			}
    		}
    		
    		public String Qp_Intro {
    			get {
    				return this._Qp_Intro;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_Intro, _Qp_Intro, value);
    				this._Qp_Intro = value;
    			}
    		}
    		
    		public Boolean Qp_IsDeleted {
    			get {
    				return this._Qp_IsDeleted;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_IsDeleted, _Qp_IsDeleted, value);
    				this._Qp_IsDeleted = value;
    			}
    		}
    		
    		public Boolean Qp_IsUse {
    			get {
    				return this._Qp_IsUse;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_IsUse, _Qp_IsUse, value);
    				this._Qp_IsUse = value;
    			}
    		}
    		
    		public String Qp_Name {
    			get {
    				return this._Qp_Name;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_Name, _Qp_Name, value);
    				this._Qp_Name = value;
    			}
    		}
    		
    		public Int32 Qp_Order {
    			get {
    				return this._Qp_Order;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_Order, _Qp_Order, value);
    				this._Qp_Order = value;
    			}
    		}
    		
    		public Int64 Qp_PID {
    			get {
    				return this._Qp_PID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_PID, _Qp_PID, value);
    				this._Qp_PID = value;
    			}
    		}
    		
    		public DateTime Qp_UpdateTime {
    			get {
    				return this._Qp_UpdateTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_UpdateTime, _Qp_UpdateTime, value);
    				this._Qp_UpdateTime = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<QuesPart>("QuesPart");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qp_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qp_ID,
    					_.Org_ID,
    					_.QP_Count,
    					_.Qp_CrtTime,
    					_.Qp_DeleteTime,
    					_.Qp_Intro,
    					_.Qp_IsDeleted,
    					_.Qp_IsUse,
    					_.Qp_Name,
    					_.Qp_Order,
    					_.Qp_PID,
    					_.Qp_UpdateTime};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qp_ID,
    					this._Org_ID,
    					this._QP_Count,
    					this._Qp_CrtTime,
    					this._Qp_DeleteTime,
    					this._Qp_Intro,
    					this._Qp_IsDeleted,
    					this._Qp_IsUse,
    					this._Qp_Name,
    					this._Qp_Order,
    					this._Qp_PID,
    					this._Qp_UpdateTime};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qp_ID))) {
    				this._Qp_ID = reader.GetInt64(_.Qp_ID);
    			}
    			if ((false == reader.IsDBNull(_.Org_ID))) {
    				this._Org_ID = reader.GetInt32(_.Org_ID);
    			}
    			if ((false == reader.IsDBNull(_.QP_Count))) {
    				this._QP_Count = reader.GetInt32(_.QP_Count);
    			}
    			if ((false == reader.IsDBNull(_.Qp_CrtTime))) {
    				this._Qp_CrtTime = reader.GetDateTime(_.Qp_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Qp_DeleteTime))) {
    				this._Qp_DeleteTime = reader.GetDateTime(_.Qp_DeleteTime);
    			}
    			if ((false == reader.IsDBNull(_.Qp_Intro))) {
    				this._Qp_Intro = reader.GetString(_.Qp_Intro);
    			}
    			if ((false == reader.IsDBNull(_.Qp_IsDeleted))) {
    				this._Qp_IsDeleted = reader.GetBoolean(_.Qp_IsDeleted);
    			}
    			if ((false == reader.IsDBNull(_.Qp_IsUse))) {
    				this._Qp_IsUse = reader.GetBoolean(_.Qp_IsUse);
    			}
    			if ((false == reader.IsDBNull(_.Qp_Name))) {
    				this._Qp_Name = reader.GetString(_.Qp_Name);
    			}
    			if ((false == reader.IsDBNull(_.Qp_Order))) {
    				this._Qp_Order = reader.GetInt32(_.Qp_Order);
    			}
    			if ((false == reader.IsDBNull(_.Qp_PID))) {
    				this._Qp_PID = reader.GetInt64(_.Qp_PID);
    			}
    			if ((false == reader.IsDBNull(_.Qp_UpdateTime))) {
    				this._Qp_UpdateTime = reader.GetDateTime(_.Qp_UpdateTime);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(QuesPart).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<QuesPart>();
    			
    			/// <summary>
    			/// 字段名：Qp_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qp_ID = new WeiSha.Data.Field<QuesPart>("Qp_ID");
    			
    			/// <summary>
    			/// 字段名：Org_ID - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Org_ID = new WeiSha.Data.Field<QuesPart>("Org_ID");
    			
    			/// <summary>
    			/// 字段名：QP_Count - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field QP_Count = new WeiSha.Data.Field<QuesPart>("QP_Count");
    			
    			/// <summary>
    			/// 字段名：Qp_CrtTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qp_CrtTime = new WeiSha.Data.Field<QuesPart>("Qp_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Qp_DeleteTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qp_DeleteTime = new WeiSha.Data.Field<QuesPart>("Qp_DeleteTime");
    			
    			/// <summary>
    			/// 字段名：Qp_Intro - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qp_Intro = new WeiSha.Data.Field<QuesPart>("Qp_Intro");
    			
    			/// <summary>
    			/// 字段名：Qp_IsDeleted - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Qp_IsDeleted = new WeiSha.Data.Field<QuesPart>("Qp_IsDeleted");
    			
    			/// <summary>
    			/// 字段名：Qp_IsUse - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Qp_IsUse = new WeiSha.Data.Field<QuesPart>("Qp_IsUse");
    			
    			/// <summary>
    			/// 字段名：Qp_Name - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Qp_Name = new WeiSha.Data.Field<QuesPart>("Qp_Name");
    			
    			/// <summary>
    			/// 字段名：Qp_Order - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Qp_Order = new WeiSha.Data.Field<QuesPart>("Qp_Order");
    			
    			/// <summary>
    			/// 字段名：Qp_PID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qp_PID = new WeiSha.Data.Field<QuesPart>("Qp_PID");
    			
    			/// <summary>
    			/// 字段名：Qp_UpdateTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Qp_UpdateTime = new WeiSha.Data.Field<QuesPart>("Qp_UpdateTime");
    		}
    	}
    }
    