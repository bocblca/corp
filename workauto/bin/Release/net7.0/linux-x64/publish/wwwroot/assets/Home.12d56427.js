import{T as v,N}from"./NavBar.84903a19.js";import{_ as k,r as x}from"./index.56f71b78.js";import{h as B,j as l,k as w,r as d,o,l as r,m as _,n as f,w as y,p as e,t as a,F as h,q as D,u as I}from"./vendor.f660479c.js";const T={name:"home",components:{Tabbar:v,NavBar:N},setup(){let s=B({fields:["bankID","bankName","parentID"],banks:[]}),c=l("\u9996\u9875"),i=l(!0),t=l("\u8FD4\u56DE");return w(async()=>{const m=await x({url:"/Wx/Getbranch",data:{branchName:"\u671D\u9633\u5206\u884C"},method:"post"});s.banks=m.data}),{mdata:s,title:c,isleftarrow:i,lefttext:t}}},j={key:0},V={key:1},g=I("\u4E3B\u8981\u6309\u94AE"),C=["fields"],S={class:"small"},q={class:"small"},F={class:"small"},H={class:"small"},E={class:"small"},G={href:"#"},L={class:"small"};function M(s,c,i,t,m,W){const u=d("NavBar"),b=d("Tabbar"),p=d("van-button");return o(),r(h,null,[s.$route.meta.isShowNavBar?(o(),r("div",j,[_(u,{title:t.title,"left-arrow":t.isleftarrow,"left-text":t.lefttext},null,8,["title","left-arrow","left-text"])])):f("",!0),s.$route.meta.isShowTabBar?(o(),r("div",V,[_(b)])):f("",!0),_(p,{type:"primary"},{default:y(()=>[g]),_:1}),e("div",null,[e("table",null,[e("tr",{fields:t.mdata.fields},[e("th",S,a(t.mdata.fields[0]),1),e("th",q,a(t.mdata.fields[1]),1),e("th",F,a(t.mdata.fields[2]),1)],8,C),(o(!0),r(h,null,D(t.mdata.banks,n=>(o(),r("tr",{key:n.bankID},[e("td",H,a(n.bankID),1),e("td",E,[e("a",G,a(n.bankName),1)]),e("td",L,a(n.parentID),1)]))),128))])])],64)}var K=k(T,[["render",M],["__scopeId","data-v-287c7710"]]);export{K as default};
