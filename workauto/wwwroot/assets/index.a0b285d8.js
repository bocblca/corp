import{c as u,a as f,r as p,o as d,b as m,d as h,e as _,i as v,s as y}from"./vendor.a8539960.js";const k=function(){const n=document.createElement("link").relList;if(n&&n.supports&&n.supports("modulepreload"))return;for(const e of document.querySelectorAll('link[rel="modulepreload"]'))r(e);new MutationObserver(e=>{for(const t of e)if(t.type==="childList")for(const o of t.addedNodes)o.tagName==="LINK"&&o.rel==="modulepreload"&&r(o)}).observe(document,{childList:!0,subtree:!0});function s(e){const t={};return e.integrity&&(t.integrity=e.integrity),e.referrerpolicy&&(t.referrerPolicy=e.referrerpolicy),e.crossorigin==="use-credentials"?t.credentials="include":e.crossorigin==="anonymous"?t.credentials="omit":t.credentials="same-origin",t}function r(e){if(e.ep)return;e.ep=!0;const t=s(e);fetch(e.href,t)}};k();const g="modulepreload",a={},E="/",L=function(n,s){return!s||s.length===0?n():Promise.all(s.map(r=>{if(r=`${E}${r}`,r in a)return;a[r]=!0;const e=r.endsWith(".css"),t=e?'[rel="stylesheet"]':"";if(document.querySelector(`link[href="${r}"]${t}`))return;const o=document.createElement("link");if(o.rel=e?"stylesheet":g,e||(o.as="script",o.crossOrigin=""),o.href=r,document.head.appendChild(o),e)return new Promise((c,l)=>{o.addEventListener("load",c),o.addEventListener("error",l)})})).then(()=>n())},$=()=>L(()=>import("./kpi.42438b30.js"),["assets/kpi.42438b30.js","assets/kpi.63fb1df9.css","assets/vendor.a8539960.js"]),b=[{path:"/kpi",name:"kpi\u4E0A\u4F20\u9875",component:$,meta:{requireAuth:!1,isShowTabBar:!1,isShowNavBar:!1,title:"Execl\u4E0A\u4F20"}},{path:"/",name:"kpi",redirect:"/kpi",meta:{requireAuth:!0,isShowTabBar:!0,isShowNavBar:!0,title:"Kpi-Excel\u4E0A\u4F20"}}],w=u({history:f({}.BASE_URL),routes:b});var S=(i,n)=>{const s=i.__vccOpts||i;for(const[r,e]of n)s[r]=e;return s};const x={name:"app"};function A(i,n,s,r,e,t){const o=p("router-view");return d(),m(o)}var B=S(x,[["render",A]]);h(B).use(_()).use(v).use(y).use(w).mount("#app");export{S as _,w as r};
