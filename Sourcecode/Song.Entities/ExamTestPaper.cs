namespace Song.Entities {
    	using System;
    	
    	
    	/// <summary>
    	/// 表名：ExamTestPaper 主键列：Etp_Id
    	/// </summary>
    	[SerializableAttribute()]
    	public partial class ExamTestPaper : WeiSha.Data.Entity {
    		
    		protected Int64 _Etp_Id;
    		
    		protected String _Acc_AccName;
    		
    		protected Int32 _Acc_Id;
    		
    		protected String _Etp_Author;
    		
    		protected Int32 _Etp_Count;
    		
    		protected DateTime _Etp_CrtTime;
    		
    		protected DateTime _Etp_DeleteTime;
    		
    		protected Int32 _Etp_Diff;
    		
    		protected Int32 _Etp_Diff2;
    		
    		protected String _Etp_FromConfig;
    		
    		protected Int32 _Etp_FromType;
    		
    		protected String _Etp_Intro;
    		
    		protected Boolean _Etp_IsBuild;
    		
    		protected Boolean _Etp_IsDeleted;
    		
    		protected Boolean _Etp_IsManual;
    		
    		protected Boolean _Etp_IsRec;
    		
    		protected Boolean _Etp_IsUse;
    		
    		protected DateTime _Etp_Lasttime;
    		
    		protected String _Etp_Logo;
    		
    		protected String _Etp_Name;
    		
    		protected Int32 _Etp_PassScore;
    		
    		protected String _Etp_Remind;
    		
    		protected Int32 _Etp_Span;
    		
    		protected String _Etp_SubName;
    		
    		protected Int32 _Etp_Total;
    		
    		protected Int32 _Etp_Type;
    		
    		protected Int32 _Org_ID;
    		
    		public Int64 Etp_Id {
    			get {
    				return this._Etp_Id;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Id, _Etp_Id, value);
    				this._Etp_Id = value;
    			}
    		}
    		
    		public String Acc_AccName {
    			get {
    				return this._Acc_AccName;
    			}
    			set {
    				this.OnPropertyValueChange(_.Acc_AccName, _Acc_AccName, value);
    				this._Acc_AccName = value;
    			}
    		}
    		
    		public Int32 Acc_Id {
    			get {
    				return this._Acc_Id;
    			}
    			set {
    				this.OnPropertyValueChange(_.Acc_Id, _Acc_Id, value);
    				this._Acc_Id = value;
    			}
    		}
    		
    		public String Etp_Author {
    			get {
    				return this._Etp_Author;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Author, _Etp_Author, value);
    				this._Etp_Author = value;
    			}
    		}
    		
    		public Int32 Etp_Count {
    			get {
    				return this._Etp_Count;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Count, _Etp_Count, value);
    				this._Etp_Count = value;
    			}
    		}
    		
    		public DateTime Etp_CrtTime {
    			get {
    				return this._Etp_CrtTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_CrtTime, _Etp_CrtTime, value);
    				this._Etp_CrtTime = value;
    			}
    		}
    		
    		public DateTime Etp_DeleteTime {
    			get {
    				return this._Etp_DeleteTime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_DeleteTime, _Etp_DeleteTime, value);
    				this._Etp_DeleteTime = value;
    			}
    		}
    		
    		public Int32 Etp_Diff {
    			get {
    				return this._Etp_Diff;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Diff, _Etp_Diff, value);
    				this._Etp_Diff = value;
    			}
    		}
    		
    		public Int32 Etp_Diff2 {
    			get {
    				return this._Etp_Diff2;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Diff2, _Etp_Diff2, value);
    				this._Etp_Diff2 = value;
    			}
    		}
    		
    		public String Etp_FromConfig {
    			get {
    				return this._Etp_FromConfig;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_FromConfig, _Etp_FromConfig, value);
    				this._Etp_FromConfig = value;
    			}
    		}
    		
    		public Int32 Etp_FromType {
    			get {
    				return this._Etp_FromType;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_FromType, _Etp_FromType, value);
    				this._Etp_FromType = value;
    			}
    		}
    		
    		public String Etp_Intro {
    			get {
    				return this._Etp_Intro;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Intro, _Etp_Intro, value);
    				this._Etp_Intro = value;
    			}
    		}
    		
    		public Boolean Etp_IsBuild {
    			get {
    				return this._Etp_IsBuild;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_IsBuild, _Etp_IsBuild, value);
    				this._Etp_IsBuild = value;
    			}
    		}
    		
    		public Boolean Etp_IsDeleted {
    			get {
    				return this._Etp_IsDeleted;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_IsDeleted, _Etp_IsDeleted, value);
    				this._Etp_IsDeleted = value;
    			}
    		}
    		
    		public Boolean Etp_IsManual {
    			get {
    				return this._Etp_IsManual;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_IsManual, _Etp_IsManual, value);
    				this._Etp_IsManual = value;
    			}
    		}
    		
    		public Boolean Etp_IsRec {
    			get {
    				return this._Etp_IsRec;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_IsRec, _Etp_IsRec, value);
    				this._Etp_IsRec = value;
    			}
    		}
    		
    		public Boolean Etp_IsUse {
    			get {
    				return this._Etp_IsUse;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_IsUse, _Etp_IsUse, value);
    				this._Etp_IsUse = value;
    			}
    		}
    		
    		public DateTime Etp_Lasttime {
    			get {
    				return this._Etp_Lasttime;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Lasttime, _Etp_Lasttime, value);
    				this._Etp_Lasttime = value;
    			}
    		}
    		
    		public String Etp_Logo {
    			get {
    				return this._Etp_Logo;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Logo, _Etp_Logo, value);
    				this._Etp_Logo = value;
    			}
    		}
    		
    		public String Etp_Name {
    			get {
    				return this._Etp_Name;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Name, _Etp_Name, value);
    				this._Etp_Name = value;
    			}
    		}
    		
    		public Int32 Etp_PassScore {
    			get {
    				return this._Etp_PassScore;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_PassScore, _Etp_PassScore, value);
    				this._Etp_PassScore = value;
    			}
    		}
    		
    		public String Etp_Remind {
    			get {
    				return this._Etp_Remind;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Remind, _Etp_Remind, value);
    				this._Etp_Remind = value;
    			}
    		}
    		
    		public Int32 Etp_Span {
    			get {
    				return this._Etp_Span;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Span, _Etp_Span, value);
    				this._Etp_Span = value;
    			}
    		}
    		
    		public String Etp_SubName {
    			get {
    				return this._Etp_SubName;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_SubName, _Etp_SubName, value);
    				this._Etp_SubName = value;
    			}
    		}
    		
    		public Int32 Etp_Total {
    			get {
    				return this._Etp_Total;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Total, _Etp_Total, value);
    				this._Etp_Total = value;
    			}
    		}
    		
    		public Int32 Etp_Type {
    			get {
    				return this._Etp_Type;
    			}
    			set {
    				this.OnPropertyValueChange(_.Etp_Type, _Etp_Type, value);
    				this._Etp_Type = value;
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
    		
    		/// <summary>
    		/// 获取实体对应的表名
    		/// </summary>
    		protected override WeiSha.Data.Table GetTable() {
    			return new WeiSha.Data.Table<ExamTestPaper>("ExamTestPaper");
    		}
    		
    		/// <summary>
    		/// 获取实体中的主键列
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetPrimaryKeyFields() {
    			return new WeiSha.Data.Field[] {
    					_.Etp_Id};
    		}
    		
    		/// <summary>
    		/// 获取列信息
    		/// </summary>
    		protected override WeiSha.Data.Field[] GetFields() {
    			return new WeiSha.Data.Field[] {
    					_.Etp_Id,
    					_.Acc_AccName,
    					_.Acc_Id,
    					_.Etp_Author,
    					_.Etp_Count,
    					_.Etp_CrtTime,
    					_.Etp_DeleteTime,
    					_.Etp_Diff,
    					_.Etp_Diff2,
    					_.Etp_FromConfig,
    					_.Etp_FromType,
    					_.Etp_Intro,
    					_.Etp_IsBuild,
    					_.Etp_IsDeleted,
    					_.Etp_IsManual,
    					_.Etp_IsRec,
    					_.Etp_IsUse,
    					_.Etp_Lasttime,
    					_.Etp_Logo,
    					_.Etp_Name,
    					_.Etp_PassScore,
    					_.Etp_Remind,
    					_.Etp_Span,
    					_.Etp_SubName,
    					_.Etp_Total,
    					_.Etp_Type,
    					_.Org_ID};
    		}
    		
    		/// <summary>
    		/// 获取列数据
    		/// </summary>
    		protected override object[] GetValues() {
    			return new object[] {
    					this._Etp_Id,
    					this._Acc_AccName,
    					this._Acc_Id,
    					this._Etp_Author,
    					this._Etp_Count,
    					this._Etp_CrtTime,
    					this._Etp_DeleteTime,
    					this._Etp_Diff,
    					this._Etp_Diff2,
    					this._Etp_FromConfig,
    					this._Etp_FromType,
    					this._Etp_Intro,
    					this._Etp_IsBuild,
    					this._Etp_IsDeleted,
    					this._Etp_IsManual,
    					this._Etp_IsRec,
    					this._Etp_IsUse,
    					this._Etp_Lasttime,
    					this._Etp_Logo,
    					this._Etp_Name,
    					this._Etp_PassScore,
    					this._Etp_Remind,
    					this._Etp_Span,
    					this._Etp_SubName,
    					this._Etp_Total,
    					this._Etp_Type,
    					this._Org_ID};
    		}
    		
    		/// <summary>
    		/// 给当前实体赋值
    		/// </summary>
    		protected override void SetValues(WeiSha.Data.IRowReader reader) {
    			if ((false == reader.IsDBNull(_.Etp_Id))) {
    				this._Etp_Id = reader.GetInt64(_.Etp_Id);
    			}
    			if ((false == reader.IsDBNull(_.Acc_AccName))) {
    				this._Acc_AccName = reader.GetString(_.Acc_AccName);
    			}
    			if ((false == reader.IsDBNull(_.Acc_Id))) {
    				this._Acc_Id = reader.GetInt32(_.Acc_Id);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Author))) {
    				this._Etp_Author = reader.GetString(_.Etp_Author);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Count))) {
    				this._Etp_Count = reader.GetInt32(_.Etp_Count);
    			}
    			if ((false == reader.IsDBNull(_.Etp_CrtTime))) {
    				this._Etp_CrtTime = reader.GetDateTime(_.Etp_CrtTime);
    			}
    			if ((false == reader.IsDBNull(_.Etp_DeleteTime))) {
    				this._Etp_DeleteTime = reader.GetDateTime(_.Etp_DeleteTime);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Diff))) {
    				this._Etp_Diff = reader.GetInt32(_.Etp_Diff);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Diff2))) {
    				this._Etp_Diff2 = reader.GetInt32(_.Etp_Diff2);
    			}
    			if ((false == reader.IsDBNull(_.Etp_FromConfig))) {
    				this._Etp_FromConfig = reader.GetString(_.Etp_FromConfig);
    			}
    			if ((false == reader.IsDBNull(_.Etp_FromType))) {
    				this._Etp_FromType = reader.GetInt32(_.Etp_FromType);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Intro))) {
    				this._Etp_Intro = reader.GetString(_.Etp_Intro);
    			}
    			if ((false == reader.IsDBNull(_.Etp_IsBuild))) {
    				this._Etp_IsBuild = reader.GetBoolean(_.Etp_IsBuild);
    			}
    			if ((false == reader.IsDBNull(_.Etp_IsDeleted))) {
    				this._Etp_IsDeleted = reader.GetBoolean(_.Etp_IsDeleted);
    			}
    			if ((false == reader.IsDBNull(_.Etp_IsManual))) {
    				this._Etp_IsManual = reader.GetBoolean(_.Etp_IsManual);
    			}
    			if ((false == reader.IsDBNull(_.Etp_IsRec))) {
    				this._Etp_IsRec = reader.GetBoolean(_.Etp_IsRec);
    			}
    			if ((false == reader.IsDBNull(_.Etp_IsUse))) {
    				this._Etp_IsUse = reader.GetBoolean(_.Etp_IsUse);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Lasttime))) {
    				this._Etp_Lasttime = reader.GetDateTime(_.Etp_Lasttime);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Logo))) {
    				this._Etp_Logo = reader.GetString(_.Etp_Logo);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Name))) {
    				this._Etp_Name = reader.GetString(_.Etp_Name);
    			}
    			if ((false == reader.IsDBNull(_.Etp_PassScore))) {
    				this._Etp_PassScore = reader.GetInt32(_.Etp_PassScore);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Remind))) {
    				this._Etp_Remind = reader.GetString(_.Etp_Remind);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Span))) {
    				this._Etp_Span = reader.GetInt32(_.Etp_Span);
    			}
    			if ((false == reader.IsDBNull(_.Etp_SubName))) {
    				this._Etp_SubName = reader.GetString(_.Etp_SubName);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Total))) {
    				this._Etp_Total = reader.GetInt32(_.Etp_Total);
    			}
    			if ((false == reader.IsDBNull(_.Etp_Type))) {
    				this._Etp_Type = reader.GetInt32(_.Etp_Type);
    			}
    			if ((false == reader.IsDBNull(_.Org_ID))) {
    				this._Org_ID = reader.GetInt32(_.Org_ID);
    			}
    		}
    		
    		public override int GetHashCode() {
    			return base.GetHashCode();
    		}
    		
    		public override bool Equals(object obj) {
    			if ((obj == null)) {
    				return false;
    			}
    			if ((false == typeof(ExamTestPaper).IsAssignableFrom(obj.GetType()))) {
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
    			public static WeiSha.Data.AllField All = new WeiSha.Data.AllField<ExamTestPaper>();
    			
    			/// <summary>
    			/// 字段名：Etp_Id - 数据类型：Int64
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Id = new WeiSha.Data.Field<ExamTestPaper>("Etp_Id");
    			
    			/// <summary>
    			/// 字段名：Acc_AccName - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Acc_AccName = new WeiSha.Data.Field<ExamTestPaper>("Acc_AccName");
    			
    			/// <summary>
    			/// 字段名：Acc_Id - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Acc_Id = new WeiSha.Data.Field<ExamTestPaper>("Acc_Id");
    			
    			/// <summary>
    			/// 字段名：Etp_Author - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Author = new WeiSha.Data.Field<ExamTestPaper>("Etp_Author");
    			
    			/// <summary>
    			/// 字段名：Etp_Count - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Count = new WeiSha.Data.Field<ExamTestPaper>("Etp_Count");
    			
    			/// <summary>
    			/// 字段名：Etp_CrtTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Etp_CrtTime = new WeiSha.Data.Field<ExamTestPaper>("Etp_CrtTime");
    			
    			/// <summary>
    			/// 字段名：Etp_DeleteTime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Etp_DeleteTime = new WeiSha.Data.Field<ExamTestPaper>("Etp_DeleteTime");
    			
    			/// <summary>
    			/// 字段名：Etp_Diff - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Diff = new WeiSha.Data.Field<ExamTestPaper>("Etp_Diff");
    			
    			/// <summary>
    			/// 字段名：Etp_Diff2 - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Diff2 = new WeiSha.Data.Field<ExamTestPaper>("Etp_Diff2");
    			
    			/// <summary>
    			/// 字段名：Etp_FromConfig - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_FromConfig = new WeiSha.Data.Field<ExamTestPaper>("Etp_FromConfig");
    			
    			/// <summary>
    			/// 字段名：Etp_FromType - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_FromType = new WeiSha.Data.Field<ExamTestPaper>("Etp_FromType");
    			
    			/// <summary>
    			/// 字段名：Etp_Intro - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Intro = new WeiSha.Data.Field<ExamTestPaper>("Etp_Intro");
    			
    			/// <summary>
    			/// 字段名：Etp_IsBuild - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Etp_IsBuild = new WeiSha.Data.Field<ExamTestPaper>("Etp_IsBuild");
    			
    			/// <summary>
    			/// 字段名：Etp_IsDeleted - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Etp_IsDeleted = new WeiSha.Data.Field<ExamTestPaper>("Etp_IsDeleted");
    			
    			/// <summary>
    			/// 字段名：Etp_IsManual - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Etp_IsManual = new WeiSha.Data.Field<ExamTestPaper>("Etp_IsManual");
    			
    			/// <summary>
    			/// 字段名：Etp_IsRec - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Etp_IsRec = new WeiSha.Data.Field<ExamTestPaper>("Etp_IsRec");
    			
    			/// <summary>
    			/// 字段名：Etp_IsUse - 数据类型：Boolean
    			/// </summary>
    			public static WeiSha.Data.Field Etp_IsUse = new WeiSha.Data.Field<ExamTestPaper>("Etp_IsUse");
    			
    			/// <summary>
    			/// 字段名：Etp_Lasttime - 数据类型：DateTime
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Lasttime = new WeiSha.Data.Field<ExamTestPaper>("Etp_Lasttime");
    			
    			/// <summary>
    			/// 字段名：Etp_Logo - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Logo = new WeiSha.Data.Field<ExamTestPaper>("Etp_Logo");
    			
    			/// <summary>
    			/// 字段名：Etp_Name - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Name = new WeiSha.Data.Field<ExamTestPaper>("Etp_Name");
    			
    			/// <summary>
    			/// 字段名：Etp_PassScore - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_PassScore = new WeiSha.Data.Field<ExamTestPaper>("Etp_PassScore");
    			
    			/// <summary>
    			/// 字段名：Etp_Remind - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Remind = new WeiSha.Data.Field<ExamTestPaper>("Etp_Remind");
    			
    			/// <summary>
    			/// 字段名：Etp_Span - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Span = new WeiSha.Data.Field<ExamTestPaper>("Etp_Span");
    			
    			/// <summary>
    			/// 字段名：Etp_SubName - 数据类型：String
    			/// </summary>
    			public static WeiSha.Data.Field Etp_SubName = new WeiSha.Data.Field<ExamTestPaper>("Etp_SubName");
    			
    			/// <summary>
    			/// 字段名：Etp_Total - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Total = new WeiSha.Data.Field<ExamTestPaper>("Etp_Total");
    			
    			/// <summary>
    			/// 字段名：Etp_Type - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Etp_Type = new WeiSha.Data.Field<ExamTestPaper>("Etp_Type");
    			
    			/// <summary>
    			/// 字段名：Org_ID - 数据类型：Int32
    			/// </summary>
    			public static WeiSha.Data.Field Org_ID = new WeiSha.Data.Field<ExamTestPaper>("Org_ID");
    		}
    	}
    }
    