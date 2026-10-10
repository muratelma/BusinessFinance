const { AppFormSheet, AppTextField, AppInlineNotice } = window.DesignSystem_02d8cd;
const gsTL = n => '₺' + Math.abs(n).toLocaleString('tr-TR',{minimumFractionDigits:2,maximumFractionDigits:2});
const gsNum = s => Number(String(s||'').replace(/\./g,'').replace(',','.'))||0;
const gsT = {
  label:{font:'var(--text-label-weight) var(--text-label-size)/var(--text-label-line) var(--font-core)',letterSpacing:'var(--text-label-tracking)',color:'var(--ink-faint)'},
  help:{margin:0,font:'var(--text-helper-size)/var(--text-helper-line) var(--font-core)',color:'var(--ink-muted)',textWrap:'pretty'},
  row:{font:'600 var(--text-row-size)/var(--text-row-line) var(--font-core)',color:'var(--ink)'},
  body:{font:'var(--text-body-size)/var(--text-body-line) var(--font-core)',color:'var(--ink-muted)'},
  money:{font:'600 var(--text-row-size)/var(--text-row-line) var(--font-core)',fontVariantNumeric:'tabular-nums',color:'var(--ink)',whiteSpace:'nowrap'},
  bodyMoney:{font:'var(--text-body-size)/var(--text-body-line) var(--font-core)',fontVariantNumeric:'tabular-nums',color:'var(--ink)',whiteSpace:'nowrap'},
  action:{all:'unset',cursor:'pointer',display:'inline-flex',alignItems:'center',gap:4,minHeight:48,font:'600 var(--text-chip-size)/1.2 var(--font-core)',color:'var(--ink)'}
};

function GsIcon({n,s=20,c='var(--ink-muted)'}){return <span className="material-symbols-outlined" aria-hidden="true" style={{fontSize:s,color:c}}>{n}</span>}
function GsBox({on}){return <span aria-hidden="true" style={{width:20,height:20,boxSizing:'border-box',borderRadius:4,border:on?'none':'2px solid var(--ink-muted)',background:on?'var(--brand)':'transparent',display:'grid',placeItems:'center'}}>{on&&<GsIcon n="check" s={18} c="var(--on-brand)"/>}</span>}
function GsRadio({on}){return <span aria-hidden="true" style={{width:20,height:20,boxSizing:'border-box',borderRadius:'50%',border:'2px solid '+(on?'var(--brand)':'var(--ink-muted)'),display:'grid',placeItems:'center'}}>{on&&<span style={{width:10,height:10,borderRadius:'50%',background:'var(--brand)'}}></span>}</span>}

/* AppSegmentRail görünümü: gri kapsül, seçili dilim beyaz */
function GsRail({value,options,onChange,label}){
  return <div role="group" aria-label={label} style={{display:'flex',padding:4,gap:4,borderRadius:999,background:'var(--surface-card-muted)',border:'1px solid var(--border)'}}>
    {options.map(o=>{const on=o.value===value;return <button key={o.value} type="button" aria-pressed={on} onClick={()=>onChange(o.value)} style={{all:'unset',flex:1,textAlign:'center',minHeight:40,lineHeight:'40px',borderRadius:999,cursor:'pointer',font:'600 14px/40px var(--font-core)',color:on?'var(--ink)':'var(--ink-muted)',background:on?'var(--surface-card)':'transparent',boxShadow:on?'inset 0 0 0 1px var(--border-strong)':'none'}}>{o.label}</button>})}
  </div>;
}

/* Satır: soldaki 48 dp alan onay kutusu; cevapsızsa soru ikonu */
function GsRow({r,v,onTap,trailing}){
  const asked=v===undefined;
  return <div style={{display:'flex',alignItems:'center',gap:'var(--space-sm)',minHeight:56}}>
    <button type="button" role="checkbox" aria-checked={asked?'mixed':!!v} aria-label={r.name} onClick={onTap} style={{all:'unset',display:'flex',alignItems:'center',gap:'var(--space-sm)',flex:1,minWidth:0,cursor:'pointer',padding:'var(--space-xs) 0'}}>
      <span style={{width:48,height:48,margin:'0 -14px',display:'grid',placeItems:'center',flex:'0 0 auto'}}>{asked?<GsIcon n="help" s={22} c="var(--ink-faint)"/>:<GsBox on={v}/>}</span>
      <span style={{flex:1,minWidth:0,display:'flex',flexDirection:'column',marginLeft:'var(--space-sm)'}}>
        <span style={{...gsT.row,overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap'}}>{r.name}</span>
        <span style={gsT.body}>{r.meta}</span>
      </span>
      <span style={gsT.money}>{gsTL(r.amount)}</span>
    </button>
    {trailing}
  </div>;
}

/* G4 sorusu: iki satırın hemen altında. Sağdaki sayı: bu çiftten kayıtlı sayılacak tutar (sunucudan) */
function GsPairQuestion({who,inv,sale,col,how,setHow,part,setPart}){
  const max=Math.min(sale,col);
  const inLabel=inv?(col<=sale?'Tahsilat faturanın içinde':'Fatura tahsilatın içinde'):(col<=sale?'Tahsilat satışın içinde':'Satış tahsilatın içinde');
  const opts=[{v:'sep',l:'İkisi ayrı ayrı',res:sale+col},{v:'in',l:inLabel,res:Math.max(sale,col)},{v:'part',l:'Bir kısmı ikisinde de var',res:gsNum(part)?sale+col-Math.min(gsNum(part),max):null}];
  return <div style={{display:'flex',flexDirection:'column',padding:'var(--space-sm) var(--space-md) var(--space-md)',borderRadius:'var(--radius-field)',background:'var(--surface-card-muted)',margin:'var(--space-xs) 0 var(--space-sm)'}}>
    <p style={{...gsT.help,color:'var(--ink)',paddingTop:'var(--space-sm)'}}>{inv?'Fatura ve tahsilatı':'Satış ve tahsilat'} yazdığınız nakit tutarda nasıl sayıldı?</p>
    <span style={{...gsT.body,fontSize:'var(--text-label-small-size)',color:'var(--ink-faint)',alignSelf:'flex-end',marginTop:'var(--space-xs)'}}>Kayıtlı sayılan</span>
    <div role="radiogroup" aria-label={who} style={{display:'flex',flexDirection:'column'}}>
      {opts.map(o=><button key={o.v} type="button" role="radio" aria-checked={how===o.v} onClick={()=>setHow(o.v)} style={{all:'unset',display:'flex',alignItems:'center',gap:'var(--space-md)',minHeight:48,cursor:'pointer'}}>
        <GsRadio on={how===o.v}/>
        <span style={{flex:1,minWidth:0,font:'var(--text-body-size)/var(--text-body-line) var(--font-core)',color:'var(--ink)'}}>{o.l}</span>
        <span style={{...gsT.body,fontVariantNumeric:'tabular-nums',whiteSpace:'nowrap'}}>{o.res!=null?gsTL(o.res):''}</span>
      </button>)}
    </div>
    {how==='part'&&<div style={{paddingLeft:36,display:'flex',flexDirection:'column',gap:'var(--space-xs)'}}>
      <AppTextField label="İkisinde de sayılan" value={part} onChange={setPart} suffix="TRY" labelBackdrop="var(--surface-card-muted)" style={{marginTop:'var(--space-sm)'}}/>
      <p style={{...gsT.help,paddingLeft:'var(--space-md)'}}>En çok {gsTL(max)}.</p>
    </div>}
  </div>;
}

/* Yazılacak özeti. variant 'block' (önerilen) | 'line' */
function GsSummary({title,place,amount,lines,pending,variant='block',foot,label=true}){
  return <div style={{display:'flex',flexDirection:'column',gap:'var(--space-xs)'}}>
    {label&&<span style={gsT.label}>Yazılacak</span>}
    <div style={{display:'flex',flexDirection:'column',padding:'var(--space-md)',borderRadius:'var(--radius-field)',background:'var(--surface-card-muted)'}}>
      <div style={{display:'flex',alignItems:'flex-start',gap:'var(--space-md)'}}>
        <span style={{flex:1,minWidth:0,display:'flex',flexDirection:'column'}}>
          <span style={{...gsT.row,fontWeight:400}}>{title}</span>
          {(()=>{const t=pending||(variant==='line'&&lines?lines.line:place);return (Array.isArray(t)?t:[t]).map((x,i)=><span key={i} style={gsT.body}>{x}</span>)})()}
        </span>
        {!pending&&<span style={{...gsT.money,color:'var(--income)'}}>+{gsTL(amount)}</span>}
      </div>
      {!pending&&variant==='block'&&lines&&<div style={{display:'flex',flexDirection:'column',gap:2,marginTop:'var(--space-sm)',paddingTop:'var(--space-sm)',borderTop:'1px solid var(--border)'}}>
        <div style={{display:'flex',justifyContent:'space-between',gap:'var(--space-md)'}}><span style={gsT.body}>{lines.inLabel||'Nakit tutarı'}</span><span style={gsT.bodyMoney}>{gsTL(lines.cash)}</span></div>
        <div style={{display:'flex',justifyContent:'space-between',gap:'var(--space-md)'}}><span style={gsT.body}>Zaten kayıtlı</span><span style={gsT.bodyMoney}>−{gsTL(lines.ded)}</span></div>
        {lines.parts&&[].concat(lines.parts).map((x,i)=><span key={i} style={{...gsT.body,fontSize:'var(--text-label-small-size)',color:'var(--ink-faint)'}}>{x}</span>)}
      </div>}
      {foot}
    </div>
  </div>;
}

function GsTop({cash,setCash,pos='',setPos,total=''}){
  return <>
    <AppTextField label="Gün" value="10 Ekim Cumartesi" labelBackdrop="var(--surface-card)" suffix={<GsIcon n="calendar_today"/>}/>
    <AppTextField label="Nakit tutarı" value={cash} onChange={setCash} suffix="TRY" labelBackdrop="var(--surface-card)"/>
    <div style={{display:'flex',flexDirection:'column'}}>
      <AppTextField label="Ziraat POS" value={pos} onChange={setPos} suffix="TRY" labelBackdrop="var(--surface-card)"/>
      <button type="button" style={{...gsT.action,alignSelf:'flex-start',paddingLeft:'var(--space-xs)'}}>Diğer POS'lar (2)<GsIcon n="expand_more" c="var(--ink)"/></button>
    </div>
    <AppTextField label="Toplam (isteğe bağlı)" value={total} suffix="TRY" labelBackdrop="var(--surface-card)"/>
  </>;
}

function GsSheet({children,canSave}){
  return <AppFormSheet title="Gün sonu" submitLabel="Gün sonunu kaydet" onSubmit={canSave?()=>{}:undefined} onCancel={()=>{}} style={{maxHeight:'none',overflow:'visible'}}>
    <div style={{display:'flex',flexDirection:'column',gap:'var(--space-md)',padding:'var(--space-sm) 0 var(--space-md)'}}>{children}
      <button type="button" style={{...gsT.action,alignSelf:'flex-start'}}>Kasayı ya da kategoriyi değiştir</button>
    </div>
  </AppFormSheet>;
}

Object.assign(window,{gsTL,gsNum,gsT,GsIcon,GsBox,GsRadio,GsRail,GsRow,GsPairQuestion,GsSummary,GsTop,GsSheet});
