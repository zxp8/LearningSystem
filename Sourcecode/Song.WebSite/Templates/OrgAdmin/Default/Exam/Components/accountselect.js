// 账号选择
//事件：
//select:当选择学员时触发，返回选中的学员
Vue.component('accountselect', {
    //accounts:已经选择的学员
    props: ['orgid', 'size', 'accounts'],
    data: function () {
        return {
            tabs: [
                { title: '单个添加', name: 'add', icon: 'e759' },
                { title: '批量添加', name: 'bat', icon: 'a04d' }],
            activeName: 'add',     //选项卡

            loading: false
        }
    },
    watch: {

    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //添加学员
        //values:该参数为学员账号的数组
        studentadd_event: function (values) {
            var tmarr = [];
            for (let j = 0; j < values.length; j++) {
                const exists = this.accounts.some(user => user.Ac_ID === values[j].Ac_ID);
                if (!exists) tmarr.push(values[j]);
            }
            this.accounts = this.accounts.concat(tmarr);
            this.$emit('select', this.accounts);
        },
    },
    template: `<div class="accountselect">
        <el-tabs v-model="activeName">
            <el-tab-pane v-for="tab in tabs" :name="tab.name">
                <span slot="label">
                    <icon v-html="'&#x'+tab.icon" medium></icon> {{tab.title}}
                </span>
            </el-tab-pane>
        </el-tabs>
       
        <!-- 新增学员到学员组（单个新增）-->
        <student_add  v-show="activeName=='add'" :orgid="orgid" :accounts="accounts" @add="studentadd_event">
        </student_add>       
        
        <!-- 批量添加学员到学员组-->
        <student_batadd  v-show="activeName=='bat'" :orgid="orgid" :accounts="accounts" @add="studentadd_event">
        </student_batadd>
       
    </div> `
});

// 新增学员到学员组（单个新增）
Vue.component('student_add', {
    props: ['stsid', 'orgid', 'accounts'],
    data: function () {
        return {
            datas: [],
            form: {
                'orgid': '', 'sortid': '', 'use': null, 'acc': '', 'name': '', 'phone': '', 'idcard': '',
                'gender': '-1', 'orderby': '', 'orderpattr': '',
                'index': 1, 'size': '10'
            },
            total: 1, //总记录数
            totalpages: 1, //总页数
            selects: [], //数据表中选中的行

            loading: false
        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                if (nv != null) {
                    var th = this;
                    th.form.orgid = nv;
                    th.loading = true;
                    //初始加载
                    window._student_add_load = window.setInterval(function () {
                        var area = $dom('.student_add');
                        if (area.length > 0 && area.height() > 0) {
                            window.clearInterval(window._student_add_load);
                            var maxheight = area.height() - 34 - 32 - 10;
                            //console.error(maxheight);
                            th.form.size = Math.round(maxheight / 30);
                            th.getdatas(1);
                        }
                    }, 100);
                }
            }, immediate: true
        }

    },
    computed: {},
    mounted: function () { },
    methods: {
        //加载数据页
        getdatas: function (index) {
            if (index != null) this.form.index = index;
            var th = this;           
            th.loading = true;
            $api.get("Account/Pager", th.form).then(function (d) {
                if (d.data.success) {
                    th.datas = d.data.result;
                    th.totalpages = Number(d.data.totalpages);
                    th.total = d.data.total;
                } else {
                    console.error(d.data.exception);
                    throw d.data.message;
                }
            }).catch(function (err) {
                alert(err);
            }).finally(() => th.loading = false);
        },
        //是否已经被选择
        isselected: function (item) {
            return this.accounts.some(user => user.Ac_ID === item.Ac_ID);
        },
        //增加学员
        add: function (item) {
            this.$emit('add', [item]);
        }
    },
    //
    template: `<div class="student_add">
            <header>
                <el-input v-model="form.name" placeholder="姓名" clearable @input="getdatas(1)"></el-input> 
                <el-input v-model="form.acc" placeholder="账号" clearable @input="getdatas(1)"></el-input> 
                <el-input v-model="form.idcard" placeholder="身份证" clearable @input="getdatas(1)"></el-input> 
                <el-input v-model="form.phone" placeholder="电话" clearable @input="getdatas(1)"></el-input> 
            </header>
            <loading v-if="loading">加载中...</loading>
            <dl class="list" v-else-if="datas.length>0">
                <dd v-for="(item,i) in datas" :index="(form.index-1) * form.size+i+1" :info="isselected(item)"> 
                    <icon class="name" size="large" :woman="item.Ac_Gender==2" :man="item.Ac_Gender==1" v-html='showsearch(item.Ac_Name,form.name)'></icon>                      
                    <span v-if="form.phone!=''" class="phone"  v-html='showsearch(item.Ac_MobiTel1,form.phone)'></span>   
                    <span v-else class="idcard"  v-html='showsearch(item.Ac_IDCardNumber,form.idcard)'></span>                  
                    <el-link class="btn" type="primary" @click="add(item)" v-if="!isselected(item)" title="添加">添加</el-link>
                    <span v-else mini>已选</span>
                </dd>
            </dl>
            <icon v-else null>没有满足条件的数据</icon>
            <el-pagination v-on:current-change="getdatas" :current-page="form.index" 
                :page-size="form.size" :pager-count="4" layout="total, prev, pager, next" :total="total">
            </el-pagination>
        </div>`
});
//批量添加学员到学员组
Vue.component('student_batadd', {
    props: ['stsid', 'orgid', 'accounts'],
    data: function () {
        return {
            datas: [],
            search_type: 'card',    //检索类型，账号acc,身份证card，手机mobi

            inputText: '',
            inputIsChange: false,        //是否有输入变化
            operstatus: 1,        //操作状态，默认1录入数据，2为解析数据
            loading: false
        }
    },
    watch: {
        //当检索类型改变时，也标识为输入变化，否则无法再次解析
        'search_type': {
            handler: function (nv, ov) {
                if (nv != ov) {
                    this.inputIsChange = true;
                }
            },
        }
    },
    computed: {
        //查询的完成数
        'query_completed': t => t.datas.filter(i => i.state != -1).length,
        //查询的有效记录数
        'query_successful': t => t.datas.filter(i => i.state == 1).length,
        //允许添加的数量
        'allow_add': function () {
            return this.datas.length - this.exists_count;
        },
        //已经存在的学员数量
        'exists_count': function () {
            let count = 0;
            for (let i = 0; i < this.datas.length; i++) {
                if (this.accounts.some(user => user.Ac_ID === this.datas[i].account.Ac_ID)) count++;
            }
            return count;
        },
        //解析中
        'parseloading': function () {
            if (this.operstatus == 1) return false;
            else {
                for (let i = 0; i < this.datas.length; i++) {
                    if (this.datas[i].state == -1) return true;
                }
                return false;
            }
        },
    },
    mounted: function () { },
    methods: {
        //解析按钮的事件 
        btnParse: function () {
            var str = $api.trim(this.inputText);
            if (str == '') return;
            if (!this.inputIsChange) {
                this.operstatus = 2;
                return;
            }

            this.datas = [];
            //解析录入的信息
            var arr = str.split("\n");
            //校验证手机号，简单校验
            if (this.search_type == "mobi") {
                var regPos = / ^\d+$/; // 非负整数 
                for (var i = 0; i < arr.length; i++) {
                    const d = arr[i].replace(/\s*/g, "");
                    if (!regPos.test(d)) arr.splice(i, 1);
                }
            }
            //校验身份证，简单校验而已
            if (this.search_type == "card") {
                var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
                for (var i = 0; i < arr.length; i++) {
                    const d = arr[i].replace(/\s*/g, "");
                    if (!reg.test(d)) arr.splice(i, 1);
                }
            }
            for (var i = 0; i < arr.length; i++) {
                arr[i] = arr[i].replace(/\s*/g, "");
                if (arr[i] == '') continue;
                //state：状态,初始为-1，账号不存在为0，存在为1，处理完成为2
                this.datas.push({ 'text': arr[i], 'account': {}, 'state': -1 });
            }
            this.operstatus = 2;
            this.inputIsChange = false;
        },
        //是否已经被选择
        isselected: function (item) {
            //console.error(item);
            return this.accounts.some(user => user.Ac_ID === item.account.Ac_ID);
        },
        //增加学员的事件
        add: function () {
            const arr = this.datas.filter(item => item.state == 1).map(item => item.account);
            if (arr.length > 0) this.$emit('add', arr);
        },
    },
    //
    template: `<div class="student_batadd">  
            <header small info>
                请在下面输入框录入学员信息，换行分隔<br />并明确录入的是：
                <el-radio-group v-model="search_type" :disabled="operstatus==2">
                    <el-radio label="acc">账号</el-radio>
                    <el-radio label="card">身份证</el-radio>
                    <el-radio label="mobi">手机号</el-radio>
                </el-radio-group>
            </header>
            <div class="btns">
                <el-button type="primary" plain v-if="operstatus==1" @click="btnParse" :disabled="parseloading">
                    <icon>&#xe83c</icon>解析录入的信息
                </el-button>
                <template v-else>
                    <el-button type="success" plain  @click="operstatus=1">
                        <icon>&#xe63d</icon>继续编辑内容
                    </el-button>
                    <el-tooltip class="item" effect="dark" placement="bottom">
                        <div slot="content">录入{{query_completed}}条信息，解析成功{{query_successful}}条<br/>
                            已经选择的账号信息有 {{exists_count}} 条<br/>
                            可供添加的账号信息有 {{allow_add}} 条
                        </div>
                        <el-button type="primary" plain @click="add" :disabled="allow_add<1" :loading="parseloading">
                            <span v-if="parseloading">正在处理...</span>
                            <template v-else>
                                <icon>&#xe6ea</icon>全部添加<span>（{{allow_add}}条）</span>
                            </template>
                        </el-button>     
                    </el-tooltip>    
                </template>       
            </div>
            <el-input type="textarea" @input="inputIsChange = true" :rows="10" placeholder="请输入内容" v-if="operstatus==1"  v-model="inputText">
            </el-input>      
            <section v-if="operstatus==2" class="accounts">
                <header small>
                    <div>#</div>
                    <div> 
                        <span v-if="search_type=='acc'">账号</span>
                        <span v-if="search_type=='card'">身份证</span>
                        <span v-if="search_type=='mobi'">手机号</span>
                        <span title="总数"> {{datas.length}} 条</span>
                    </div>
                    <div>
                        <el-tooltip effect="dark" :content="'查询完成 '+ query_completed+' 条，有效 '+query_successful+' 条'" placement="bottom">
                            <span> 完成 <b :primary="query_successful>0">{{query_successful}}</b> / {{query_completed}}</span>
                        </el-tooltip>   
                    </div>
                </header>
                <dl>                
                    <dd v-for="(item,index) in datas" :index="index" small :info="isselected(item)"> 
                        <div class="order">{{index+1}}</div>
                        <div class="text">{{item.text}}</div>
                        <div class="result">
                            <accountselect_queryaccount :item="item" :text="item.text" :type="search_type"></accountselect_queryaccount>
                        </div>
                    </dd>
                </dl> 
            </section>           
        </div>`
});
//账号信息的获取
Vue.component('accountselect_queryaccount', {
    //item:录入项,
    //text:录入的内容，可能是账号或身份证号
    //type:搜索类型
    props: ["item", "text", "type"],
    data: function () {
        return {
            data: {},
            state: -1,
            loading: true
        }
    },
    watch: {
        'text': {
            handler: function (nv, ov) {
                this.state = this.item.state;
                this.getaccount();
            }, immediate: true, deep: true
        }
    },
    computed: {},
    mounted: function () { },
    methods: {
        //获取账号信息
        getaccount: function () {
            var th = this;
            th.loading = true;
            //
            var apiurl = "Account/ForAcc", para = {};
            if (this.type == 'acc') {
                apiurl = "Account/ForAcc";
                para = { "acc": this.text };
            }
            if (this.type == 'card') {
                apiurl = "Account/ForIDCard";
                para = { "card": this.text };
            }
            if (this.type == 'mobi') {
                apiurl = "Account/ForMobi";
                para = { "phone": this.text };
            }
            $api.cache(apiurl, para).then(function (req) {
                if (req.data.success) {
                    th.data = req.data.result;
                    th.item.account = req.data.result;
                    th.item.state = th.state = 1;
                } else {
                    th.data = null;
                    th.item.account = null;
                    th.item.state = th.state = 0;
                    console.error(req.data.exception);
                    throw req.data.message;
                }
            }).catch(err => console.error(err)).finally(() => {
                th.loading = false;
                th.$emit('loaded', true);
            });
        }
    },
    template: `<span title="学员信息">
            <span v-if="state==-1" class="el-icon-loading"></span>
            <span v-else-if="state==0"><el-tag type="info">不存在</el-tag></span>
            <span v-else-if="state==1">
                <icon size="medium" v-if="data.Ac_Gender==2" woman>{{data.Ac_Name}}</icon>
                <icon size="medium"  v-if="data.Ac_Gender==1" man>{{data.Ac_Name}}</icon>  
                <span v-if="!data.Ac_IsUse" info>（已经禁用）</span>
            </span>
        </span> `
});