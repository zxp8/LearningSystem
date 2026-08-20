namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：Exam_Accounts 主键列：Ea_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class Exam_Accounts : WeiSha.Data.Entity {
    		
    		protected Int64 _Ea_ID;
    		
    		protected Int32 _Ac_ID;
    		
    		protected String _Exam_UID;
    		
    		public Int64 Ea_ID {
    			get {
    				return this._Ea_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Ea_ID, _Ea_ID, value);
    				this._Ea_ID = value;
    			}
    		}
    		
    		public Int32 Ac_ID {
    			get {
    				return this._Ac_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Ac_ID, _Ac_ID, value);
    				this._Ac_ID = value;
    			}
    		}
    		
    		public String Exam_UID {
    			get {
    				return this._Exam_UID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Exam_UID, _Exam_UID, value);
    				this._Exam_UID = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<Exam_Accounts>("Exam_Accounts");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Ea_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Ea_ID,
    					_.Ac_ID,
    					_.Exam_UID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Ea_ID,
    					this._Ac_ID,
    					this._Exam_UID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Ea_ID))) {
    				this._Ea_ID = reader.GetInt64(_.Ea_ID);
    			}
    			if ((false == reader.IsDBNull(_.Ac_ID))) {
    				this._Ac_ID = reader.GetInt32(_.Ac_ID);
    			}
    			if ((false == reader.IsDBNull(_.Exam_UID))) {
    				this._Exam_UID = reader.GetString(_.Exam_UID);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(Exam_Accounts).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<Exam_Accounts>();
    			
    			/// <summary>
    			/// 字段名：Ea_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Ea_ID = new WeiSha.Data.Field<Exam_Accounts>("Ea_ID");
    			
    			/// <summary>
    			/// 字段名：Ac_ID - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Ac_ID = new WeiSha.Data.Field<Exam_Accounts>("Ac_ID");
    			
    			/// <summary>
    			/// 字段名：Exam_UID - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Exam_UID = new WeiSha.Data.Field<Exam_Accounts>("Exam_UID");
    		}
    	}
    }
    