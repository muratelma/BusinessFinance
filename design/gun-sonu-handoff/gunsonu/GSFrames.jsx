const GS_SALE={id:'s',kind:'sale',name:'Satış geliri',meta:'Nakit · Dükkan kasası',amount:250};
const GS_CARD={id:'p',kind:'card',name:'Ziraat POS satışı',meta:'Kartla · Ziraat POS',amount:800};
const GS_AHMET={id:'a',kind:'col',who:'Ahmet Bakkal',name:'Ahmet Bakkal',meta:'Tahsilat · Dükkan kasası',amount:300};
const GS_MEHMET={id:'m',kind:'col',who:'Mehmet Usta',name:'Mehmet Usta',meta:'Tahsilat · Dükkan kasası',amount:120};
const GS_AYSE={id:'y',kind:'credit',who:'Ayşe Terzi',name:'Ayşe Terzi',meta:'Veresiye satış',amount:450};
const GS_DAY=[GS_SALE,GS_CARD,GS_AHMET,GS_MEHMET,GS_AYSE];
const GS_G4=[{id:'ms',kind:'credit',who:'Mehmet Usta',name:'Mehmet Usta',meta:'Veresiye satış',amount:500},{id:'mc',kind:'col',who:'Mehmet Usta',name:'Mehmet Usta',meta:'Tahsilat · Dükkan kasası',amount:300}];
const GS_INV=[{id:'fi',kind:'credit',inv:true,who:'Sentetik fatura',name:'Sentetik fatura',meta:'Alacak faturası',amount:400},{id:'fc',kind:'col',inv:true,who:'Sentetik fatura',name:'Sentetik fatura',meta:'Tahsilat · Dükkan kasası',amount:400}];
const GS_TWO=[...GS_G4,{id:'as',kind:'credit',who:'Ahmet Bakkal',name:'Ahmet Bakkal',meta:'Veresiye satış',amount:300},{id:'ac',kind:'col',who:'Ahmet Bakkal',name:'Ahmet Bakkal',meta:'Tahsilat · Dükkan kasası',amount:500}];

function gsBig(rs){return window.GS_STRESS?rs.map(r=>({...r,amount:r.amount*333.33,name:r.name==='Ahmet Bakkal'?'Ahmet Bakkal ve Oğulları Gıda Ltd.':r.name})):rs}
function GunSonu({records:rec0=GS_DAY,initCash='1.670',initPos='',initAns={s:true,p:true},initHow={},initPart={},summary='block',bulk='rail'}){
  const records=gsBig(rec0);
  if(window.GS_STRESS){if(initCash)initCash='999.999';if(initPos)initPos='999.999';}
  const [cash,setCash]=React.useState(initCash);
  const [pos,setPos]=React.useState(initPos);
  const [ans,setAns]=React.useState(initAns);
  const [how,setHowMap]=React.useState(initHow);
  const [part,setPartMap]=React.useState(initPart);
  const c=gsNum(cash),k=gsNum(pos);
  const pre=records.filter(r=>r.kind==='sale'||r.kind==='card').filter(r=>r.kind==='card'||c>0);
  const asked=records.filter(r=>r.kind==='col'||r.kind==='credit');
  const showAsked=c>0&&asked.length>0;
  const set=(id,v)=>setAns({...ans,[id]:v});
  const tap=r=>set(r.id,ans[r.id]===undefined?true:!ans[r.id]);
  const all=v=>{const n={...ans};asked.forEach(r=>n[r.id]=v);setAns(n)};
  const railVal=asked.every(r=>ans[r.id]===true)?'in':asked.every(r=>ans[r.id]===false)?'out':null;
  const open=showAsked?asked.filter(r=>ans[r.id]===undefined).length:0;
  const whos=[...new Set(asked.map(r=>r.who))];
  const pairs=whos.map(w=>{const s=asked.find(r=>r.who===w&&r.kind==='credit'),t=asked.find(r=>r.who===w&&r.kind==='col');return s&&t&&ans[s.id]&&ans[t.id]?{w,s,t}:null}).filter(Boolean);
  const pairOpen=pairs.filter(p=>!how[p.w]||(how[p.w]==='part'&&!gsNum(part[p.w]))).length;
  const on=r=>(r.kind==='sale'||r.kind==='card')?ans[r.id]:(showAsked&&ans[r.id]===true);
  const sum=kd=>records.filter(r=>r.kind===kd&&on(r)).reduce((a,r)=>a+r.amount,0);
  let adj=0;const adjL=[];
  pairs.forEach(p=>{const h=how[p.w];const x=h==='in'?Math.min(p.s.amount,p.t.amount):h==='part'?Math.min(gsNum(part[p.w]),p.s.amount,p.t.amount):0;if(x){adj+=x;adjL.push('− '+gsTL(x)+' ikisinde de')}});
  const ded=sum('sale')+sum('credit')+sum('col')-adj;
  const parts=[[sum('sale'),'satış'],[sum('credit'),'veresiye satış'],[sum('col'),'tahsilat']].filter(x=>x[0]).map(x=>gsTL(x[0])+' '+x[1]).join(' + ');
  const pending=!c?'Nakit tutarı yazılınca hesaplanır.':open?`${open} kayıt için seçim yapılınca hesaplanır.`:pairOpen?'Yukarıdaki soru cevaplanınca hesaplanır.':null;
  const lines={cash:c,ded,parts:ded?[parts,...adjL]:null,line:`${gsTL(c)} − zaten kayıtlı ${gsTL(ded)}`};
  const cardDed=sum('card'), net=k-cardDed;
  const showCash=c>0||!k;

  const renderAsked=r=>{
    const pair=pairs.find(p=>p.t.id===r.id);
    return <React.Fragment key={r.id}>
      <GsRow r={r} v={ans[r.id]} onTap={()=>tap(r)}/>
      {bulk==='row'&&<div style={{paddingLeft:40,marginBottom:'var(--space-sm)'}}><GsRail label={r.name} value={ans[r.id]===true?'in':ans[r.id]===false?'out':null} onChange={v=>set(r.id,v==='in')} options={[{value:'in',label:'İçinde'},{value:'out',label:'Değil'}]}/></div>}
      {pair&&<GsPairQuestion who={pair.w} inv={pair.s.inv} sale={pair.s.amount} col={pair.t.amount} how={how[pair.w]} setHow={v=>setHowMap({...how,[pair.w]:v})} part={part[pair.w]||''} setPart={v=>setPartMap({...part,[pair.w]:v})}/>}
    </React.Fragment>;
  };
  return <GsSheet canSave={!(showCash&&pending)}>
    <GsTop cash={cash} setCash={setCash} pos={pos} setPos={setPos}/>
    {(pre.length>0||showAsked)&&<div style={{display:'flex',flexDirection:'column'}}>
      <span style={gsT.label}>Gün içinde girilenler</span>
      <p style={gsT.help}>İşaretli kayıtlar yazılan tutarın içindedir; tekrar kaydedilmez.</p>
      <div style={{marginTop:'var(--space-xs)'}}>{pre.map(r=><GsRow key={r.id} r={r} v={!!ans[r.id]} onTap={()=>set(r.id,!ans[r.id])}/>)}</div>
      {showAsked&&<>
        <div style={{display:'flex',flexDirection:'column',gap:'var(--space-sm)',marginTop:pre.length?'var(--space-sm)':0,paddingTop:pre.length?'var(--space-sm)':0,borderTop:pre.length?'1px solid var(--border)':'none'}}>
          <div style={{display:'flex',alignItems:'center',justifyContent:'space-between',gap:'var(--space-sm)',flexWrap:'wrap'}}>
            <span style={{font:'var(--text-body-size)/var(--text-body-line) var(--font-core)',color:'var(--ink)'}}>Bunlar yazdığınız nakit tutarın içinde mi?</span>
            {bulk==='row'&&<button type="button" onClick={()=>all(true)} style={{...gsT.action,minHeight:40}}>Hepsi içinde</button>}
          </div>
          {bulk==='rail'&&<GsRail label="Hepsi için" value={railVal} onChange={v=>all(v==='in')} options={[{value:'in',label:'Hepsi içinde'},{value:'out',label:'Hiçbiri'}]}/>}
          {bulk==='rail'&&<p style={gsT.help}>Tek tek değiştirmek için satıra dokunun.</p>}
        </div>
        <div style={{marginTop:'var(--space-xs)'}}>{asked.map(renderAsked)}</div>
      </>}
    </div>}
    <div style={{display:'flex',flexDirection:'column',gap:'var(--space-xs)'}}>
      <span style={gsT.label}>Yazılacak</span>
      {showCash&&<GsSummary label={false} title="Yeni nakit satış" place="Dükkan kasası · Satış geliri" amount={c-ded} lines={lines} pending={pending} variant={summary}/>}
      {k>0&&<GsSummary label={false} title="Ziraat POS satışı" place={[`Komisyon ${gsTL(net*0.02)}`,'12 Eki Pazartesi hesaba geçer']} amount={net} lines={cardDed?{cash:k,ded:cardDed,inLabel:'Kart tutarı',parts:null}:null} variant={summary}/>}
    </div>
  </GsSheet>;
}

function GunSonuPos(){
  const S=window.GS_STRESS,pos=S?999999:500,tot=S?1999999:1800;
  const fmt=n=>n.toLocaleString('tr-TR');
  return <GsSheet canSave={true}>
    <GsTop cash="" setCash={()=>{}} pos={fmt(pos)} total={fmt(tot)}/>
    <AppInlineNotice message={`Yalnız kart satışı kaydedilir. Toplamla arasındaki ${gsTL(tot-pos)} kaydedilmez.`}/>
    <GsSummary title="Ziraat POS satışı" place={[`Komisyon ${gsTL(pos*0.02)}`,'12 Eki Pazartesi hesaba geçer']} amount={pos}/>
  </GsSheet>;
}

function GunSonuPairPanel(){
  const [how,setHow]=React.useState('part');const [part,setPart]=React.useState('200');
  return <AppFormSheet title="Mehmet Usta" rule="Veresiye satış ₺500,00 ve tahsilat ₺300,00 bugün işaretli." submitLabel="Tamam" onSubmit={()=>{}} onCancel={()=>{}}>
    <div style={{paddingBottom:'var(--space-md)'}}><GsPairQuestion who="" sale={500} col={300} how={how} setHow={setHow} part={part} setPart={setPart}/></div>
  </AppFormSheet>;
}

Object.assign(window,{GunSonu,GunSonuPos,GunSonuPairPanel,GS_G4,GS_DAY,GS_INV,GS_TWO,GS_CARD});
