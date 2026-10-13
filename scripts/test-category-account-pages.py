"""Yalnız sentetik veriyle, ayrı loopback Release API'de brif 5/6 denemesi."""
import argparse
import json
import uuid
from decimal import Decimal as D
from pathlib import Path
from urllib.request import Request, urlopen
from urllib.error import HTTPError
from urllib.parse import urlencode, urlparse

parser = argparse.ArgumentParser()
parser.add_argument('--url', default='http://127.0.0.1:5291')
parser.add_argument('--output', default='scratch/category-account-api-results.json')
args = parser.parse_args()
parsed = urlparse(args.url)
assert parsed.hostname in ('127.0.0.1', 'localhost') and parsed.port != 5284, 'Use an isolated loopback API port.'
token = None
checks = []

def call(method, path, body=None, status=None):
    headers = {'Content-Type': 'application/json'}
    if token: headers['Authorization'] = 'Bearer ' + token
    request = Request(args.url + path, data=json.dumps(body).encode() if body is not None else None,
                      headers=headers, method=method)
    try:
        with urlopen(request, timeout=60) as response:
            code, content = response.status, response.read()
    except HTTPError as error:
        code, content = error.code, error.read()
    if status is not None:
        assert code == status, f'{method} {path}: expected {status}, got {code}'
    else:
        error_code = json.loads(content).get('code') if content.startswith(b'{') else 'no-json'
        assert 200 <= code < 300, f'{method} {path}: got {code}, code={error_code}'
    return json.loads(content) if content else None

def eq(label, value, expected):
    assert value == expected, f'{label}: {value!r} != {expected!r}'
    checks.append({'check': label, 'actual': str(value), 'expected': str(expected), 'passed': True})

def money(label, value, expected): eq(label, D(value), D(str(expected)))
def get(path, **query): return call('GET', path + ('?' + urlencode(query) if query else ''))
def post(path, body): return call('POST', path, body)
suffix = uuid.uuid4().hex
password = 'Synthetic-PageFixture-2026!'
post('/api/v1/auth/register', {'email': f'category-account-{suffix}@example.test', 'password': password, 'hasBusiness': True})
token = post('/api/v1/auth/login', {'email': f'category-account-{suffix}@example.test', 'password': password})['accessToken']
categories = {c['name']: c for c in get('/api/v1/categories')['items']}
roles = {c['role']: c for c in categories.values() if c['role']}
eq('Four roles', len(roles), 4)
eq('All role scopes open', all(c['defaultScope'] is None for c in roles.values()), True)

# İleri tarihli kayıt yasaktır. Brif Ekim'i aynı gün numaralarıyla Eylül'e
# taşınır; bir tek tutar değiştirilmez. Önceki ay Ağustos'tur.
def day(n): return f'2026-09-{n:02d}'
def account(name, kind, opening, scope='business'):
    return post('/api/v1/accounts', {'name': name, 'type': kind, 'currency': 'TRY', 'openingBalance': str(opening), 'defaultScope': scope})
bank = account('Ziraat Vadesiz', 'bank', 21600)
cash = account('Dükkan Kasası', 'cash', 12254)
other = account('Garanti Vadesiz', 'bank', 4500)
wallet = account('Cüzdan', 'cash', 4000, 'personal')
inactive = account('Eski Kasa', 'cash', 0)
call('PUT', f'/api/v1/accounts/{inactive["id"]}', {'name': 'Eski Kasa', 'isActive': False, 'defaultScope': 'business'})
card = post('/api/v1/credit-cards', {'name': 'Bonus Kart', 'limit': '20000', 'currency': 'TRY', 'statementClosingDay': 25, 'paymentDueDay': 5, 'defaultScope': 'business'})
def tx(name, amount, on, acc, description, kind='expense'):
    return post('/api/v1/transactions', {'accountId': acc['id'], 'categoryId': categories[name]['id'], 'amount': str(amount), 'currency': 'TRY', 'type': kind, 'scope': 'business', 'transactionDate': day(on), 'description': description})
def charge(name, amount, on, description):
    return post(f'/api/v1/credit-cards/{card["id"]}/charges', {'categoryId': categories[name]['id'], 'amount': str(amount), 'currency': 'TRY', 'scope': 'business', 'chargeDate': day(on), 'description': description})
person = post('/api/v1/counterparties', {'name': 'Test Toptancı'})
post(f'/api/v1/counterparties/{person["id"]}/charges', {'direction': 'payable', 'amount': '2500', 'currency': 'TRY', 'categoryId': categories['Ticari mal alımı']['id'], 'chargeDate': day(3), 'description': 'Vadeli alım'})
tx('Ticari mal alımı', 4200, 5, bank, 'Toptancı ödemesi')
charge('Ticari mal alımı', 3180, 4, 'Kırtasiye toptan')
tx('Ticari mal alımı', 2600, 2, cash, 'Hal alışverişi')
tx('İşyeri kirası', 6500, 1, bank, 'Ekim kirası')
tx('Personel giderleri', 4200, 15, cash, 'Personel ücreti')
tx('Kasa farkı', 150, 18, cash, 'Kasa farkı')
charge('İşyeri faturaları', 750, 12, 'Doğalgaz faturası')
bill = post('/api/v1/obligations', {'direction': 'payable', 'amount': '1110', 'currency': 'TRY', 'categoryId': categories['İşyeri faturaları']['id'], 'issueDate': day(3), 'dueDate': day(20), 'description': 'Elektrik faturası'})
post(f'/api/v1/obligations/{bill["id"]}/settlement', {'accountId': bank['id'], 'settlementDate': day(20)})
def debt(name, direction, principal, total, pay_on, acc):
    item = post('/api/v1/debts', {'counterpartyName': name, 'direction': direction, 'scope': 'business', 'principal': str(principal), 'totalRepayment': str(total), 'currency': 'TRY', 'sourceType': 'cash', 'openingAccountId': acc['id'], 'startDate': '2026-08-01', 'firstDueDate': day(pay_on), 'installmentCount': 1, 'description': None, 'asOfDate': day(30)})
    return post(f'/api/v1/debts/{item["id"]}/installments/1/pay', {'accountId': acc['id'], 'paymentDate': day(pay_on), 'asOfDate': day(30)})
debt('Örnek Banka', 'payable', 2550, 3000, 5, bank)
debt('Ahmet Usta', 'payable', 850, 1000, 20, bank)
debt('Kemal Bey', 'receivable', 1650, 2000, 12, cash)
for n, amount in [(6, 4600), (14, 4900), (25, 5300)]: tx('Satış geliri', amount, n, cash, 'Nakit satış', 'income')
for name, n, amount in [('Mehmet Usta', 6, 2000), ('Ayşe Terzi', 14, 2500)]:
    p = post('/api/v1/counterparties', {'name': name})
    post(f'/api/v1/counterparties/{p["id"]}/charges', {'direction': 'receivable', 'amount': str(amount), 'currency': 'TRY', 'categoryId': categories['Satış geliri']['id'], 'chargeDate': day(n), 'description': 'Veresiye satış'})
tx('Hizmet geliri', 3000, 10, bank, 'Web sitesi işi', 'income')
for n, amount, fee, depday, deposited in [(6, 6000, 233, 7, 5767), (14, 7500, 291, 15, 7169), (25, 8400, 326, None, None)]:
    s = post('/api/v1/pos-settlements', {'accountId': bank['id'], 'categoryId': categories['Satış geliri']['id'], 'grossAmount': str(amount), 'currency': 'TRY', 'settlementDate': day(n), 'expectedTransferDate': day(n+1), 'commissionAmount': str(fee), 'commissionCategoryId': roles['bank-fee']['id']})
    if depday:
        post('/api/v1/pos-deposits', {'clientRequestId': str(uuid.uuid4()), 'settlementIds': [s['id']], 'depositedAmount': str(deposited), 'depositDate': day(depday), 'deductionCategoryId': roles['bank-fee']['id']})
post('/api/v1/transfers', {'sourceAccountId': cash['id'], 'destinationAccountId': bank['id'], 'amount': '12000', 'currency': 'TRY', 'transferDate': day(16), 'description': None})
transfer_body = {'sourceAccountId': bank['id'], 'destinationAccountId': other['id'], 'amount': '10000', 'currency': 'TRY', 'transferDate': day(21), 'description': None, 'feeAmount': '50'}
preview = post('/api/v1/transfers/preview', transfer_body)
money('Transfer preview total', preview['totalOutflow'], 10050)
eq('Transfer preview scope', preview['resolvedScope'], 'business')
transfer = post('/api/v1/transfers', transfer_body)
parts = get(f'/api/v1/financial-activities/transfer/{transfer["id"]}/parts')['items']
money('Transfer detail fee', parts[0]['amount'], 50)
eq('Transfer detail category', parts[0]['categoryId'], roles['bank-fee']['id'])
post(f'/api/v1/credit-cards/{card["id"]}/payments', {'accountId': bank['id'], 'amount': '3000', 'currency': 'TRY', 'paymentDate': day(24), 'description': None})
post(f'/api/v1/counterparties/{person["id"]}/payments', {'direction': 'payable', 'amount': '2500', 'currency': 'TRY', 'accountId': bank['id'], 'paymentDate': day(26)})
tx('Banka ve POS komisyonu', 200, 28, bank, 'Hesap işletim ücreti')

q = {'year': 2026, 'month': 9, 'scope': 'business'}
expense = get('/api/v1/category-summary', **q)
money('Brif 5 expense', expense['total'], 26930)
eq('Expense category count', expense['categoryCount'], 7)
money('Expense slices equal total', sum(D(s['amount']) for s in expense['categories']), 26930)
money('Expense sources equal total', sum(D(s['amount']) for s in expense['sources'] if s['key'] != 'all'), 26930)
for source, total in [('account:'+bank['id'], 12440), ('account:'+cash['id'], 6950), ('card:'+card['id'], 3930), ('deferred', 3610)]:
    value=get('/api/v1/category-summary', **q, source=source)
    money('Expense source '+source.split(':')[0], value['total'], total)
income = get('/api/v1/category-summary', **q, direction='income')
money('Brif 5 income', income['total'], 44550)
money('Income slices equal total', sum(D(s['amount']) for s in income['categories']), 44550)
money('Income sources equal total', sum(D(s['amount']) for s in income['sources'] if s['key'] != 'all'), 44550)
for summary in (expense, income):
    for s in summary['categories']:
        page = get(f'/api/v1/categories/{s["id"]}/page', **q, direction=summary['direction'])
        money('Category rows '+s['name'], sum(abs(D(i['amount'])) for i in page['items']), s['amount'])
        eq('Category untruncated '+s['name'], page['hasMore'], False)
goods = get(f'/api/v1/categories/{categories["Ticari mal alımı"]["id"]}/page', **q, source='account:'+bank['id'])
money('Filtered goods', goods['summary']['total'], 4200)
money('Whole goods', goods['wholeTotal'], 12480)
eq('Goods person intersection', [p['key'] for p in goods['people']], ['all', 'none'])
bill_page = get(f'/api/v1/categories/{categories["İşyeri faturaları"]["id"]}/page', **q)
eq('Bill settled state', next(i for i in bill_page['items'] if i['state'])['state'], 'settled')
commissions = get(f'/api/v1/categories/{roles["bank-fee"]["id"]}/page', **q)
eq('Four kinds in bank fee category', sorted(set(i['part'] for i in commissions['items'] if i['part'])), ['commission','deduction','fee'])
page = get(f'/api/v1/accounts/{bank["id"]}/page', year=2026, month=9)
for field, expected in [('inflow',27936), ('outflow',31560), ('net',-3624), ('balance',21376), ('currentBalance',21376)]: money('Brif 6 '+field,page[field],expected)
eq('Brif 6 rows',page['totalCount'],13)
money('Account incoming rows',sum(D(i['amount']) for i in page['items'] if D(i['amount'])>0),27936)
money('Account outgoing rows',sum(-D(i['amount']) for i in page['items'] if D(i['amount'])<0),31560)
money('Transit summary',page['transit']['total'],8074)
transit=get(f'/api/v1/accounts/{bank["id"]}/in-transit')
money('Transit rows equal summary',sum(D(i['amount']) for i in transit['items']),8074)
for direction,total in [('in',27936),('out',31560)]:
    selected = get(f'/api/v1/accounts/{bank["id"]}/page',year=2026,month=9,direction=direction)
    money('Direction menu '+direction,sum(abs(D(o['amount'])) for o in selected['options'] if o['key']!='all'),total)
selected=get(f'/api/v1/accounts/{bank["id"]}/page',year=2026,month=9,direction='out',selection='out:payment:5')
money('Debt payment selection',selected['selectionTotal'],4000)
eq('Debt selection rows',selected['totalCount'],2)
months=get(f'/api/v1/accounts/{bank["id"]}/months',year=2026)
money('Year selector equals account balance',months['months'][8]['amount'],21376)
overview=get('/api/v1/accounts/overview')
for field,expected in [('total',48330),('cashTotal',12454),('bankTotal',35876),('inactiveTotal',0)]:money('Brif 6 overview '+field,overview[field],expected)
money('Overview groups equal total',D(overview['cashTotal'])+D(overview['bankTotal'])+D(overview['inactiveTotal']),48330)
call('DELETE',f'/api/v1/transactions/{transfer["feeTransactionId"]}',status=409)
call('DELETE',f'/api/v1/transfers/{transfer["id"]}')
after=get(f'/api/v1/accounts/{bank["id"]}/page',year=2026,month=9)
money('Cancel transfer and fee restores source',after['currentBalance'],31426)
money('Cancel transfer and fee removes recognized fee',get('/api/v1/category-summary',**q)['total'],26880)

results={'scenario':'Brif 5 and 6, same day numbers in September (future record rule)',
         'api':args.url,'checks':checks,'passed':len(checks),'differences':[
             'Cash inflow also includes the 2000 receivable installment; H11 omits that money.',
             'The single transit sale totals 8074; H1 uses the four-sale 28300 total with a single-sale row.'
         ]}
Path(args.output).write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
print(f'{len(checks)} synthetic API checks passed; results: {args.output}')
