namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：QuesCollect 主键列：Qcl_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class QuesCollect : WeiSha.Data.Entity {
    		
    		protected Int64 _Qcl_ID;
    		
    		protected Int64 _Acc_ID;
    		
    		protected DateTime? _Qcl_CrtTime;
    		
    		protected Int64 _Qus_ID;
    		
    		public Int64 Qcl_ID {
    			get {
    				return this._Qcl_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qcl_ID, _Qcl_ID, value);
    				this._Qcl_ID = value;
    			}
    		}
    		
    		public Int64 Acc_ID {
    			get {
    				return this._Acc_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Acc_ID, _Acc_ID, value);
    				this._Acc_ID = value;
    			}
    		}
    		
    		public DateTime? Qcl_CrtTime {
    			get {
    				return this._Qcl_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qcl_CrtTime, _Qcl_CrtTime, value);
    				this._Qcl_CrtTime = value;
    			}
    		}
    		
    		public Int64 Qus_ID {
    			get {
    				return this._Qus_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qus_ID, _Qus_ID, value);
    				this._Qus_ID = value;
    			}
    		}
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<QuesCollect>("QuesCollect");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qcl_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qcl_ID,
    					_.Acc_ID,
    					_.Qcl_CrtTime,
    					_.Qus_ID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qcl_ID,
    					this._Acc_ID,
    					this._Qcl_CrtTime,
    					this._Qus_ID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qcl_ID))) {
    				this._Qcl_ID = reader.GetInt64(_.Qcl_ID);
    			}
    			if ((false == reader.IsDBNull(_.Acc_ID))) {
    				this._Acc_ID = reader.GetInt64(_.Acc_ID);
    			}
    			if ((false == reader.IsDBNull(_.Qcl_CrtTime))) {
    				this._Qcl_CrtTime = reader.GetDateTime(_.Qcl_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Qus_ID))) {
    				this._Qus_ID = reader.GetInt64(_.Qus_ID);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(QuesCollect).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<QuesCollect>();
    			
    			/// <summary>
    			/// 字段名：Qcl_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qcl_ID = new WeiSha.Data.Field<QuesCollect>("Qcl_ID");
    			
    			/// <summary>
    			/// 字段名：Acc_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Acc_ID = new WeiSha.Data.Field<QuesCollect>("Acc_ID");
    			
    			/// <summary>
    			/// 字段名：Qcl_CrtTime - 数据类型：DateTime(可空)
    			/// </summary>
    			public static WeiSha.Data.Field Qcl_CrtTime = new WeiSha.Data.Field<QuesCollect>("Qcl_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Qus_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qus_ID = new WeiSha.Data.Field<QuesCollect>("Qus_ID");
    		}
    	}
    }
    