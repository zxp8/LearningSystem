namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：Questions_QKnl 主键列：Qqk_ID
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class Questions_QKnl : WeiSha.Data.Entity {
    		
    		protected Int64 _Qqk_ID;
    		
    		protected Int64 _Qk_ID;
    		
    		protected Int64 _Qus_ID;
    		
    		public Int64 Qqk_ID {
    			get {
    				return this._Qqk_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qqk_ID, _Qqk_ID, value);
    				this._Qqk_ID = value;
    			}
    		}
    		
    		public Int64 Qk_ID {
    			get {
    				return this._Qk_ID;
    			}
    			set {
    				this.OnPropertyValueChange(_.Qk_ID, _Qk_ID, value);
    				this._Qk_ID = value;
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
    			return new WeiSha.Data.Table<Questions_QKnl>("Questions_QKnl");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qqk_ID};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Qqk_ID,
    					_.Qk_ID,
    					_.Qus_ID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Qqk_ID,
    					this._Qk_ID,
    					this._Qus_ID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Qqk_ID))) {
    				this._Qqk_ID = reader.GetInt64(_.Qqk_ID);
    			}
    			if ((false == reader.IsDBNull(_.Qk_ID))) {
    				this._Qk_ID = reader.GetInt64(_.Qk_ID);
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
    			if ((false == typeof(Questions_QKnl).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<Questions_QKnl>();
    			
    			/// <summary>
    			/// 字段名：Qqk_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qqk_ID = new WeiSha.Data.Field<Questions_QKnl>("Qqk_ID");
    			
    			/// <summary>
    			/// 字段名：Qk_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qk_ID = new WeiSha.Data.Field<Questions_QKnl>("Qk_ID");
    			
    			/// <summary>
    			/// 字段名：Qus_ID - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Qus_ID = new WeiSha.Data.Field<Questions_QKnl>("Qus_ID");
    		}
    	}
    }
    