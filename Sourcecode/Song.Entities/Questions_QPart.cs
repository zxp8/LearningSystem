namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：Questions_QPart 主键列：Qqp_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class Questions_QPart : WeiSha.Data.Entity {
    		
    		protected Int64 _Qqp_ID;
    		
    		protected Int64 _Qp_ID;
    		
    		protected Int64 _Qus_ID;
    		
    		public Int64 Qqp_ID {
    			get {
    				return this._Qqp_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qqp_ID, _Qqp_ID, value);
    				this._Qqp_ID = value;
    			}
    		}
    		
    		public Int64 Qp_ID {
    			get {
    				return this._Qp_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qp_ID, _Qp_ID, value);
    				this._Qp_ID = value;
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
    			return new WeiSha.Data.Table<Questions_QPart>("Questions_QPart");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qqp_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qqp_ID,
    					_.Qp_ID,
    					_.Qus_ID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qqp_ID,
    					this._Qp_ID,
    					this._Qus_ID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qqp_ID))) {
    				this._Qqp_ID = reader.GetInt64(_.Qqp_ID);
    			}
    			if ((false == reader.IsDBNull(_.Qp_ID))) {
    				this._Qp_ID = reader.GetInt64(_.Qp_ID);
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
    			if ((false == typeof(Questions_QPart).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<Questions_QPart>();
    			
    			/// <summary>
    			/// 字段名：Qqp_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qqp_ID = new WeiSha.Data.Field<Questions_QPart>("Qqp_ID");
    			
    			/// <summary>
    			/// 字段名：Qp_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qp_ID = new WeiSha.Data.Field<Questions_QPart>("Qp_ID");
    			
    			/// <summary>
    			/// 字段名：Qus_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qus_ID = new WeiSha.Data.Field<Questions_QPart>("Qus_ID");
    		}
    	}
    }
    