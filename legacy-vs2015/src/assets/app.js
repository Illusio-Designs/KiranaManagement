/* KiranaManagement — shared app helpers (no framework) */
var KA = (function () {
  var API = ''; // same origin

  function token() { return localStorage.getItem('kirana_token'); }
  function role() { return localStorage.getItem('kirana_role'); }
  function uname() { return localStorage.getItem('kirana_name') || ''; }
  function storeId() { return localStorage.getItem('kirana_store') || ''; }
  function setSession(d) {
    localStorage.setItem('kirana_token', d.token);
    localStorage.setItem('kirana_role', d.role);
    localStorage.setItem('kirana_name', d.fullName || '');
    localStorage.setItem('kirana_store', d.storeId || '');
  }
  function logout() { localStorage.removeItem('kirana_token'); localStorage.removeItem('kirana_role');
    localStorage.removeItem('kirana_name'); localStorage.removeItem('kirana_store'); location.href = 'login.aspx'; }
  function requireRole(roles) {
    if (!token() || roles.indexOf(role()) < 0) { location.href = 'login.aspx'; return false; }
    return true;
  }

  async function api(path, method, body) {
    var headers = { 'Content-Type': 'application/json' };
    if (token()) headers['X-Auth-Token'] = token();
    var res = await fetch(API + path, { method: method || 'GET', headers: headers,
      body: body ? JSON.stringify(body) : undefined });
    if (res.status === 401) { logout(); return; }
    var text = await res.text();
    var data; try { data = text ? JSON.parse(text) : null; } catch (e) { data = text; }
    if (!res.ok) throw (data && (data.error || data.message)) || ('HTTP ' + res.status);
    return data;
  }

  function money(n) { return '\u20B9' + (Number(n) || 0).toFixed(2); }

  function toast(msg, type) {
    var wrap = document.getElementById('ka-toast');
    if (!wrap) { wrap = document.createElement('div'); wrap.id = 'ka-toast'; document.body.appendChild(wrap); }
    var t = document.createElement('div'); t.className = 'toast ' + (type || ''); t.textContent = msg;
    wrap.appendChild(t); setTimeout(function () { t.remove(); }, 3200);
  }

  /* ---- cart (localStorage) ---- */
  var cart = {
    get: function () { try { return JSON.parse(localStorage.getItem('kirana_cart')) || []; } catch (e) { return []; } },
    save: function (c) { localStorage.setItem('kirana_cart', JSON.stringify(c)); updateCartBadges(); },
    add: function (item) {
      var c = cart.get(); var f = c.filter(function (x) { return x.variantId === item.variantId; })[0];
      if (f) f.qty += item.qty; else c.push(item); cart.save(c);
    },
    setQty: function (id, qty) { var c = cart.get(); c.forEach(function (x) { if (x.variantId === id) x.qty = qty; });
      cart.save(c.filter(function (x) { return x.qty > 0; })); },
    remove: function (id) { cart.save(cart.get().filter(function (x) { return x.variantId !== id; })); },
    clear: function () { cart.save([]); },
    count: function () { return cart.get().reduce(function (s, x) { return s + x.qty; }, 0); },
    subtotal: function () { return cart.get().reduce(function (s, x) { return s + x.price * x.qty; }, 0); }
  };
  function updateCartBadges() {
    Array.prototype.forEach.call(document.querySelectorAll('[data-cart-count]'), function (el) {
      el.textContent = cart.count();
    });
  }

  function shopLogout() { localStorage.removeItem('kirana_token'); localStorage.removeItem('kirana_role');
    localStorage.removeItem('kirana_name'); localStorage.removeItem('kirana_store'); location.href = 'shop.aspx'; }

  function renderAccountNav() {
    Array.prototype.forEach.call(document.querySelectorAll('[data-acct]'), function (el) {
      if (token() && role() === 'Customer') {
        el.innerHTML = '<a class="navlink" href="orders.aspx">My orders</a> ' +
          '<a class="navlink" href="#" onclick="KA.shopLogout();return false;">Hi ' + (uname() || 'you') + ' · Logout</a>';
      } else {
        el.innerHTML = '<a class="navlink" href="login.aspx">Login</a> <a class="navlink" href="signup.aspx">Sign up</a>';
      }
    });
  }

  /* ---- dashboard shell (green, collapsible sidebar + topbar + footer) ---- */
  function toggleSide() {
    var s = document.getElementById('ka-side'); if (!s) return;
    s.classList.toggle('collapsed');
    try { localStorage.setItem('ka_side_collapsed', s.classList.contains('collapsed') ? '1' : '0'); } catch (e) {}
  }
  function shell(opts) {
    // opts: { kind:'store'|'admin', title, active, links:[{key,label,icon,href}] }
    var nav = opts.links.map(function (l) {
      return '<a href="' + l.href + '" data-tip="' + l.label + '" class="' + (l.key === opts.active ? 'active' : '') + '">' +
        '<span class="ic-emo">' + (l.icon || '•') + '</span> <span class="lbl3">' + l.label + '</span></a>';
    }).join('');
    var initials = (uname() || 'U').substring(0, 1).toUpperCase();
    var collapsed = false; try { collapsed = localStorage.getItem('ka_side_collapsed') === '1'; } catch (e) {}
    document.body.classList.add('ka-app');
    document.body.innerHTML =
      '<aside class="ka-side' + (collapsed ? ' collapsed' : '') + '" id="ka-side">' +
        '<div class="ka-side-inner">' +
          '<div class="sb-head">' +
            '<div class="sb-brand"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span><span class="bt">Kirana</span></div>' +
            '<button type="button" class="sb-toggle" title="Collapse sidebar" onclick="KA.toggleSide()">' +
              '<svg class="ic" viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="16" rx="2.5"/><path d="M9 4v16"/></svg></button>' +
          '</div>' +
          '<div class="ka-navgroup"><nav>' + nav + '</nav></div>' +
          '<div class="sb-foot">v1.0 · KiranaManagement</div>' +
        '</div>' +
      '</aside>' +
      '<div class="ka-main">' +
        '<div class="ka-top">' +
          '<h1>' + (opts.title || '') + '</h1>' +
          '<div class="ka-user"><span class="ka-avatar">' + initials + '</span><span>' + (uname() || '') + '</span>' +
          '<button class="btn btn-outline btn-sm" onclick="KA.logout()">Logout</button></div>' +
        '</div>' +
        '<main class="ka-content" id="ka-content"></main>' +
        '<footer class="ka-foot">© 2026 KiranaManagement — grocery platform · Mansi, Hiral &amp; Zigma</footer>' +
      '</div>';
    return document.getElementById('ka-content');
  }

  document.addEventListener('DOMContentLoaded', function () { updateCartBadges(); renderAccountNav(); });

  return { api: api, token: token, role: role, uname: uname, storeId: storeId, setSession: setSession,
    logout: logout, shopLogout: shopLogout, requireRole: requireRole, money: money, toast: toast, cart: cart,
    shell: shell, toggleSide: toggleSide, updateCartBadges: updateCartBadges, renderAccountNav: renderAccountNav };
})();
