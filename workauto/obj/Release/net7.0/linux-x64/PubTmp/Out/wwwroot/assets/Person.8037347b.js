import{T as c,N as m}from"./NavBar.84903a19.js";import{_ as f}from"./index.56f71b78.js";import{r as a,o as t,l as r,m as s,n,F as p}from"./vendor.f660479c.js";const _={name:"person",components:{Tabbar:c,NavBar:m},data(){return{title:"",isleftarrow:!0,lefttext:"\u8FD4\u56DE"}},created(){this.title=this.$route.meta.title}},u={key:0},d={key:1};function v(o,b,B,N,e,h){const l=a("NavBar"),i=a("Tabbar");return t(),r(p,null,[o.$route.meta.isShowNavBar?(t(),r("div",u,[s(l,{title:e.title,"left-arrow":e.isleftarrow,"left-text":e.lefttext},null,8,["title","left-arrow","left-text"])])):n("",!0),o.$route.meta.isShowTabBar?(t(),r("div",d,[s(i)])):n("",!0)],64)}var k=f(_,[["render",v]]);export{k as default};
