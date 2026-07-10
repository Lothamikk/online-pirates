namespace DarkPortal.Services;

public class LocaleService
{
	public string CurrentLang { get; private set; } = "ru";
	public event Action? OnChanged;

	public void SetLanguage(string lang) { CurrentLang = lang; OnChanged?.Invoke(); }

	public string T(string key) => key switch
	{
		// Меню
		"menu.tools" => L("🛠️ Инструменты", "🛠️ Tools", "🛠️ Werkzeuge", "🛠️ Outils", "🛠️ Herramientas"),
		"menu.feed" => L("📡 Досье", "📡 Dossier", "📡 Dossier", "📡 Dossier", "📡 Dosier"),
		"menu.crypto" => L("🔐 Шифратор", "🔐 Encryptor", "🔐 Verschlüsselung", "🔐 Chiffreur", "🔐 Cifrador"),
		"menu.profile" => L("👤 Профиль", "👤 Profile", "👤 Profil", "👤 Profil", "👤 Perfil"),
		"menu.settings" => L("⚙️ Настройки", "⚙️ Settings", "⚙️ Einstellungen", "⚙️ Paramètres", "⚙️ Ajustes"),
		"menu.login" => L("Войти", "Login", "Anmelden", "Connexion", "Iniciar sesión"),
		"menu.logout" => L("Выйти", "Logout", "Abmelden", "Déconnexion", "Cerrar sesión"),
		"menu.title" => L("Online Pirates // Обучение", "Online Pirates // Training", "Online Pirates // Schulung", "Online Pirates // Formation", "Online Pirates // Formación"),

		// Главная
		"home.title" => L("ОПЕРАТИВНАЯ БЕЗОПАСНОСТЬ (OPSEC)", "OPERATIONAL SECURITY (OPSEC)", "OPERATIONSSICHERHEIT (OPSEC)", "SÉCURITÉ OPÉRATIONNELLE (OPSEC)", "SEGURIDAD OPERATIVA (OPSEC)"),
		"home.subtitle" => L("Цикл статей для анонимности в сети.", "Articles to become a ghost online.", "Artikel, um ein Geist zu werden.", "Articles pour devenir un fantôme.", "Artículos para ser un fantasma."),
		"home.index" => L("Индекс анонимности", "Anonymity Index", "Anonymitätsindex", "Indice d'anonymat", "Índice de anonimato"),
		"home.guest" => L("Пройди первый урок.", "Complete the first lesson.", "Absolviere die erste Lektion.", "Termine la première leçon.", "Completa la primera lección."),

		// Уроки 1-4
		"lesson.1" => L("1. Ты — товар", "1. You are the product", "1. Du bist die Ware", "1. Tu es le produit", "1. Eres el producto"),
		"lesson.1.desc" => L("Как BigTech строит твой портрет.", "How BigTech builds your profile.", "Wie BigTech dein Profil erstellt.", "Comment BigTech construit ton profil.", "Cómo BigTech construye tu perfil."),
		"lesson.2" => L("2. Браузерные войны", "2. Browser Wars", "2. Browserkriege", "2. Guerres de navigateurs", "2. Guerras de navegadores"),
		"lesson.2.desc" => L("Firefox, контейнеры, отпечатки.", "Firefox, containers, fingerprints.", "Firefox, Container, Fingerabdrücke.", "Firefox, conteneurs, empreintes.", "Firefox, contenedores, huellas."),
		"lesson.3" => L("3. Маска для сети", "3. Network Mask", "3. Netzwerkmaske", "3. Masque réseau", "3. Máscara de red"),
		"lesson.3.desc" => L("Сравниваем VPN протоколы.", "Comparing VPN protocols.", "VPN-Protokolle vergleichen.", "Comparer les protocoles VPN.", "Comparando protocolos VPN."),
		"lesson.4" => L("4. Луковый слой", "4. Onion Layer", "4. Zwiebelschicht", "4. Couche Oignon", "4. Capa Cebolla"),
		"lesson.4.desc" => L("Квест: пронеси документы через Tor.", "Quest: sneak docs through Tor.", "Quest: Dokumente durch Tor.", "Quête: docs via Tor.", "Misión: docs por Tor."),

		// Уроки 5-10
		"lesson.5" => L("5. Даркнет-рынки", "5. Darknet Markets", "5. Darknet-Märkte", "5. Marchés Darknet", "5. Mercados Darknet"),
		"lesson.5.desc" => L("Как работают, мифы и безопасность.", "How they work, myths and safety.", "Wie sie funktionieren.", "Fonctionnement, sécurité.", "Cómo funcionan, seguridad."),
		"lesson.6" => L("6. PGP-шифрование", "6. PGP Encryption", "6. PGP-Verschlüsselung", "6. Chiffrement PGP", "6. Cifrado PGP"),
		"lesson.6.desc" => L("Создание ключей, подпись, шифрование.", "Keys, signing, encryption.", "Schlüssel, Signatur.", "Clés, signature.", "Claves, firma."),
		"lesson.7" => L("7. SIM-swap защита", "7. SIM-swap Protection", "7. SIM-Swap-Schutz", "7. Protection SIM-swap", "7. Protección SIM-swap"),
		"lesson.7.desc" => L("Как защитить номер телефона от захвата.", "Protect your phone number.", "Schütze deine Rufnummer.", "Protéger ton numéro.", "Proteger tu número."),
		"lesson.8" => L("8. Фишинг и социнженерия", "8. Phishing & Social Engineering", "8. Phishing & Social Engineering", "8. Phishing & Ingénierie sociale", "8. Phishing & Ingeniería social"),
		"lesson.8.desc" => L("Как распознать обман. Мини-тест.", "Spot scams. Mini-test.", "Betrug erkennen.", "Repérer arnaques.", "Detectar estafas."),
		"lesson.9" => L("9. Криптовалюта и анонимность", "9. Crypto & Anonymity", "9. Krypto & Anonymität", "9. Crypto & Anonymat", "9. Cripto & Anonimato"),
		"lesson.9.desc" => L("Monero, миксеры, анонимные платежи.", "Monero, mixers, anonymous payments.", "Monero, Mixer.", "Monero, mixeurs.", "Monero, mixers."),
		"lesson.10" => L("10. Полный OPSEC-гайд", "10. Full OPSEC Guide", "10. OPSEC-Guide", "10. Guide OPSEC", "10. Guía OPSEC"),
		"lesson.10.desc" => L("Итоговый чек-лист из 12 пунктов.", "Final 12-point checklist.", "12-Punkte-Checkliste.", "Check-list 12 points.", "Checklist 12 puntos."),

		// Досье
		"feed.title" => L("ДОСЬЕ", "DOSSIER", "DOSSIER", "DOSSIER", "DOSIER"),
		"feed.subtitle" => L("Живые новости.", "Live news.", "Live-News.", "Actualités.", "Noticias."),
		"feed.all" => L("Все", "All", "Alle", "Tout", "Todo"),
		"feed.leaks" => L("Утечки", "Leaks", "Lecks", "Fuite", "Fugas"),
		"feed.threats" => L("Угрозы", "Threats", "Bedrohungen", "Menaces", "Amenazas"),
		"feed.vuln" => L("Уязвимости", "Vulnerabilities", "Schwachstellen", "Vulnérabilités", "Vulnerabilidades"),
		"feed.loading" => L("Загрузка...", "Loading...", "Laden...", "Chargement...", "Cargando..."),
		"feed.refresh" => L("Обновить новости", "Refresh news", "News aktualisieren", "Actualiser les news", "Actualizar noticias"),
		"feed.refreshing" => L("Обновляем...", "Refreshing...", "Aktualisiere...", "Actualisation...", "Actualizando..."),
		"feed.empty" => L("Новостей пока нет.", "No news yet.", "Noch keine News.", "Pas encore de news.", "Sin noticias aún."),

		// Настройки
		"settings.language" => L("Язык", "Language", "Sprache", "Langue", "Idioma"),
		"settings.choose" => L("Выбери язык", "Choose language", "Sprache wählen", "Choisir la langue", "Elegir idioma"),
		"settings.saved" => L("Готово!", "Done!", "Fertig!", "Fait!", "¡Listo!"),

		"level.beginner" => L("Новичок", "Beginner", "Anfänger", "Débutant", "Principiante"),
		"level.medium" => L("Средний", "Medium", "Mittel", "Intermédiaire", "Intermedio"),
		"level.advanced" => L("Продвинутый", "Advanced", "Fortgeschritten", "Avancé", "Avanzado"),
		"button.enter" => L("Вход", "Enter", "Eintritt", "Entrée", "Entrar"),

		// Урок 1
		"data.warning" => L("Данные локальны.", "Data is local.", "Daten sind lokal.", "Données locales.", "Datos locales."),
		"data.build" => L("Собери профиль", "Build profile", "Profil erstellen", "Construire profil", "Construir perfil"),
		"data.name" => L("Имя", "Name", "Name", "Nom", "Nombre"),
		"data.age" => L("Возраст", "Age", "Alter", "Âge", "Edad"),
		"data.dog" => L("Собака", "Dog", "Hund", "Chien", "Perro"),
		"data.car" => L("Машина", "Car", "Auto", "Voiture", "Coche"),
		"data.smart" => L("Умный дом", "Smart home", "Smart Home", "Maison connectée", "Hogar inteligente"),
		"data.vpn" => L("VPN", "VPN", "VPN", "VPN", "VPN"),
		"data.evaluate" => L("Оценить", "Evaluate", "Bewerten", "Évaluer", "Evaluar"),
		"data.result" => L("Результат", "Result", "Ergebnis", "Résultat", "Resultado"),
		"data.worth" => L("ты стоишь", "you are worth", "bist du wert", "tu vaux", "vales"),
		"data.per1000" => L("за 1000 показов", "per 1000 views", "pro 1000 Aufrufe", "pour 1000 vues", "por 1000 vistas"),
		"data.monthly" => L("В месяц трекеры получают", "Monthly trackers get", "Monatlich bekommen Tracker", "Par mois les trackers ont", "Mensualmente trackers obtienen"),
		"data.price" => L("Плата за бесплатные сервисы.", "Price of free services.", "Preis für kostenlose Dienste.", "Prix des services gratuits.", "Precio de servicios gratuitos."),
		"data.privacy" => L("Приватность стоит денег", "Privacy is worth money", "Privatsphäre ist Geld wert", "La vie privée vaut de l'argent", "La privacidad vale dinero"),

		// Урок 2
		"browser.intro" => L("Браузер — предатель.", "Browser is a traitor.", "Browser ist Verräter.", "Navigateur traître.", "Navegador traidor."),
		"browser.test" => L("Что сливает браузер?", "What leaks?", "Was leakt?", "Que fuit?", "¿Qué filtra?"),
		"browser.label" => L("Твой браузер", "Your browser", "Dein Browser", "Ton navigateur", "Tu navegador"),
		"browser.ext" => L("Расширения", "Extensions", "Erweiterungen", "Extensions", "Extensiones"),
		"browser.none" => L("Ничего", "Nothing", "Nichts", "Rien", "Nada"),
		"browser.all" => L("Всё + контейнеры", "All + containers", "Alles + Container", "Tout + conteneurs", "Todo + contenedores"),
		"browser.incognito" => L("Инкогнито", "Incognito", "Inkognito", "Incognito", "Incógnito"),
		"browser.cookies" => L("Чищу куки", "Clear cookies", "Cookies löschen", "Effacer cookies", "Limpiar cookies"),
		"browser.check" => L("Проверить", "Check", "Prüfen", "Vérifier", "Verificar"),
		"browser.result" => L("Результат", "Result", "Ergebnis", "Résultat", "Resultado"),
		"browser.unique" => L("Уникальность", "Uniqueness", "Einzigartigkeit", "Unicité", "Unicidad"),

		// Урок 3
		"vpn.alert" => L("VPN — первый рубеж обороны.", "VPN is the first line of defense.", "VPN ist die erste Verteidigungslinie.", "Le VPN est la première défense.", "VPN es la primera defensa."),
		"vpn.choose" => L("Выбери протокол", "Choose protocol", "Protokoll wählen", "Choisir protocole", "Elegir protocolo"),
		"vpn.desc" => L("От протокола зависит скорость и безопасность.", "Speed and security depend on protocol.", "Geschwindigkeit hängt vom Protokoll ab.", "La vitesse dépend du protocole.", "Velocidad depende del protocolo."),
		"vpn.label" => L("VPN Протокол", "VPN Protocol", "VPN-Protokoll", "Protocole VPN", "Protocolo VPN"),
		"vpn.modern" => L("современный", "modern", "modern", "moderne", "moderno"),
		"vpn.tested" => L("проверенный", "tested", "getestet", "testé", "probado"),
		"vpn.mobile" => L("мобильный", "mobile", "mobil", "mobile", "móvil"),
		"vpn.old" => L("устарел", "outdated", "veraltet", "obsolète", "obsoleto"),
		"vpn.no" => L("не использовать!", "do not use!", "nicht nutzen!", "ne pas utiliser!", "¡no usar!"),
		"vpn.country" => L("Страна сервера", "Server country", "Server-Land", "Pays du serveur", "País del servidor"),
		"vpn.ch" => L("Швейцария", "Switzerland", "Schweiz", "Suisse", "Suiza"),
		"vpn.pa" => L("Панама", "Panama", "Panama", "Panama", "Panamá"),
		"vpn.ic" => L("Исландия", "Iceland", "Island", "Islande", "Islandia"),
		"vpn.us" => L("США (14 глаз)", "USA (14 eyes)", "USA (14 Augen)", "USA (14 yeux)", "EEUU (14 ojos)"),
		"vpn.ru" => L("Россия (СОРМ)", "Russia (SORM)", "Russland (SORM)", "Russie (SORM)", "Rusia (SORM)"),
		"vpn.double" => L("Double Hop", "Double Hop", "Double Hop", "Double Hop", "Double Hop"),
		"vpn.kill" => L("Kill Switch", "Kill Switch", "Kill Switch", "Kill Switch", "Kill Switch"),
		"vpn.compare" => L("Сравнение", "Comparison", "Vergleich", "Comparaison", "Comparación"),
		"vpn.protocol" => L("Протокол", "Protocol", "Protokoll", "Protocole", "Protocolo"),
		"vpn.speed" => L("Скорость", "Speed", "Geschwindigkeit", "Vitesse", "Velocidad"),
		"vpn.security" => L("Безопасность", "Security", "Sicherheit", "Sécurité", "Seguridad"),
		"vpn.stability" => L("Стабильность", "Stability", "Stabilität", "Stabilité", "Estabilidad"),
		"vpn.doublewarn" => L("Double Hop снизит скорость на 30-50%", "Double Hop reduces speed by 30-50%", "Double Hop reduziert Geschwindigkeit um 30-50%", "Double Hop réduit la vitesse de 30-50%", "Double Hop reduce velocidad 30-50%"),
		"vpn.select" => L("Выбери протокол для сравнения.", "Select a protocol to compare.", "Wähle ein Protokoll zum Vergleich.", "Choisis un protocole.", "Elige un protocolo."),
		"vpn.server" => L("Сервер", "Server", "Server", "Serveur", "Servidor"),
		"vpn.ping" => L("Пинг", "Ping", "Ping", "Ping", "Ping"),

		// Урок 4
		"onion.alert" => L("Квест: пройди путь анонимного источника.", "Quest: become an anonymous source.", "Quest: Werde eine anonyme Quelle.", "Quête: deviens une source anonyme.", "Misión: fuente anónima."),
		"onion.journo" => L("Ты — журналист", "You are a journalist", "Du bist Journalist", "Tu es journaliste", "Eres periodista"),
		"onion.intro" => L("У тебя секретные документы. USB с Tor в кармане.", "Secret docs. USB with Tor.", "Geheime Dokumente. USB mit Tor.", "Docs secrets. USB avec Tor.", "Docs secretos. USB con Tor."),
		"onion.actions" => L("Действия", "Actions", "Aktionen", "Actions", "Acciones"),
		"onion.tor" => L("Загрузить Tor с мостами", "Load Tor with bridges", "Tor mit Bridges laden", "Charger Tor avec bridges", "Cargar Tor con bridges"),
		"onion.wifi" => L("Отправить через обычный Wi-Fi", "Send via regular Wi-Fi", "Über normales WLAN senden", "Envoyer via Wi-Fi normal", "Enviar por Wi-Fi normal"),
		"onion.right" => L("Правильно!", "Correct!", "Richtig!", "Correct!", "¡Correcto!"),
		"onion.rightdesc" => L("Tor с мостами маскирует трафик.", "Tor with bridges masks traffic.", "Tor maskiert Traffic.", "Tor masque le trafic.", "Tor enmascara tráfico."),
		"onion.rightalert" => L("Тройное шифрование. Три узла.", "Triple encryption.", "Dreifache Verschlüsselung.", "Triple chiffrement.", "Triple cifrado."),
		"onion.next" => L("Что дальше?", "What next?", "Was nun?", "Et ensuite?", "¿Ahora qué?"),
		"onion.proton" => L("ProtonMail (.onion)", "ProtonMail (.onion)", "ProtonMail (.onion)", "ProtonMail (.onion)", "ProtonMail (.onion)"),
		"onion.dropbox" => L("Dropbox через Tor", "Dropbox via Tor", "Dropbox über Tor", "Dropbox via Tor", "Dropbox por Tor"),
		"onion.fail" => L("Провал!", "Fail!", "Fehlschlag!", "Échec!", "¡Fallo!"),
		"onion.faildesc" => L("Обычный Wi-Fi + Gmail. Камеры, логи.", "Regular Wi-Fi + Gmail.", "Normales WLAN + Gmail.", "Wi-Fi normal + Gmail.", "Wi-Fi normal + Gmail."),
		"onion.failalert" => L("Личность раскрыта.", "Identity revealed.", "Identität aufgedeckt.", "Identité révélée.", "Identidad revelada."),
		"onion.retry" => L("Попробовать снова", "Try again", "Erneut versuchen", "Réessayer", "Intentar de nuevo"),
		"onion.perfect" => L("Идеальная анонимность!", "Perfect anonymity!", "Perfekte Anonymität!", "Anonymat parfait!", "¡Anonimato perfecto!"),
		"onion.perfectdesc" => L(".onion + E2E шифрование.", ".onion + E2E encryption.", ".onion + E2E.", ".onion + chiffrement E2E.", ".onion + cifrado E2E."),
		"onion.perfectalert" => L("Документы доставлены.", "Documents delivered.", "Dokumente zugestellt.", "Documents livrés.", "Documentos entregados."),
		"onion.done" => L("Квест пройден!", "Quest complete!", "Quest abgeschlossen!", "Quête terminée!", "¡Misión completa!"),
		"onion.risky" => L("Рискованно!", "Risky!", "Riskant!", "Risqué!", "¡Arriesgado!"),
		"onion.riskydesc" => L("Dropbox просит телефон.", "Dropbox asks phone.", "Dropbox will Telefon.", "Dropbox demande tél.", "Dropbox pide tel."),
		"onion.riskyalert" => L("Избегай сервисов с KYC.", "Avoid KYC services.", "Vermeide KYC-Dienste.", "Évite les services KYC.", "Evita servicios KYC."),
		"onion.other" => L("Другой путь", "Other path", "Anderer Weg", "Autre chemin", "Otro camino"),

		// Урок 5
		"darknet.alert" => L("Информация для ознакомления.", "Info only.", "Nur zur Info.", "Info seulement.", "Solo info."),
		"darknet.what" => L("Что такое даркнет?", "What is darknet?", "Was ist Darknet?", "Qu'est-ce que le darknet?", "¿Qué es darknet?"),
		"darknet.whatdesc" => L("Скрытая сеть поверх интернета. Доступ через Tor.", "Hidden network. Access via Tor.", "Verstecktes Netzwerk.", "Réseau caché via Tor.", "Red oculta via Tor."),
		"darknet.safety" => L("Безопасность", "Safety", "Sicherheit", "Sécurité", "Seguridad"),
		"darknet.safetydesc" => L("Используй Tor, VPN, не свети личные данные.", "Use Tor, VPN, hide personal data.", "Tor, VPN nutzen.", "Utilise Tor, VPN.", "Usa Tor, VPN."),
		"darknet.risks" => L("Риски", "Risks", "Risiken", "Risques", "Riesgos"),
		"darknet.risksdesc" => L("Мошенники, выходные узлы, полиция.", "Scammers, exit nodes, police.", "Betrüger, Exit-Knoten.", "Arnaqueurs, police.", "Estafadores, policía."),
		"darknet.tips" => L("Советы", "Tips", "Tipps", "Conseils", "Consejos"),
		"darknet.tipsdesc" => L("PGP для общения, Monero для платежей.", "PGP for chat, Monero for payments.", "PGP für Chat, Monero.", "PGP pour chat, Monero.", "PGP para chat, Monero."),

		// Урок 6
		"pgp.alert" => L("PGP — шифрование с открытым ключом.", "PGP is public-key encryption.", "PGP ist Public-Key.", "PGP est un standard.", "PGP es cifrado público."),
		"pgp.what" => L("Что такое PGP?", "What is PGP?", "Was ist PGP?", "Qu'est-ce que PGP?", "¿Qué es PGP?"),
		"pgp.whatdesc" => L("Асимметричное шифрование: публичный и приватный ключ.", "Asymmetric encryption.", "Asymmetrische Verschlüsselung.", "Chiffrement asymétrique.", "Cifrado asimétrico."),
		"pgp.how" => L("Как создать ключ", "How to create a key", "Schlüssel erstellen", "Créer une clé", "Crear una clave"),
		"pgp.howdesc" => L("Gpg4win, Kleopatra, ProtonMail.", "Gpg4win, Kleopatra, ProtonMail.", "Gpg4win, Kleopatra.", "Gpg4win, Kleopatra.", "Gpg4win, Kleopatra."),
		"pgp.sign" => L("Цифровая подпись", "Digital signature", "Digitale Signatur", "Signature numérique", "Firma digital"),
		"pgp.signdesc" => L("Подтверждает, что сообщение от тебя.", "Proves message is from you.", "Beweist Nachricht von dir.", "Prouve message de toi.", "Prueba mensaje tuyo."),
		"pgp.verify" => L("Проверка подписи", "Verify signature", "Signatur prüfen", "Vérifier signature", "Verificar firma"),
		"pgp.verifydesc" => L("Получатель проверяет твоим публичным ключом.", "Recipient verifies with your public key.", "Empfänger prüft mit Schlüssel.", "Vérifie avec clé publique.", "Verifica con clave pública."),

		// Урок 7
		"sim.alert" => L("SIM-swap — опасный взлом. Теряешь всё за 5 минут.", "SIM-swap is dangerous. Lose everything in 5 min.", "SIM-Swap ist gefährlich.", "SIM-swap dangereux.", "SIM-swap peligroso."),
		"sim.what" => L("Что такое SIM-swap?", "What is SIM-swap?", "Was ist SIM-Swap?", "Qu'est-ce que SIM-swap?", "¿Qué es SIM-swap?"),
		"sim.whatdesc" => L("Мошенник перевыпускает твою SIM и получает доступ к SMS.", "Scammer reissues your SIM.", "Betrüger bekommt deine SIM.", "Arnaqueur réémet ta SIM.", "Estafador reemite tu SIM."),
		"sim.how" => L("Как это работает?", "How does it work?", "Wie funktioniert es?", "Comment ça marche?", "¿Cómo funciona?"),
		"sim.howdesc" => L("Звонок оператору → социнженерия → перевыпуск SIM.", "Call operator → social engineering.", "Anruf → neue SIM.", "Appel → nouvelle SIM.", "Llamada → nueva SIM."),
		"sim.protect" => L("Как защититься?", "How to protect?", "Wie schützen?", "Comment se protéger?", "¿Cómo protegerse?"),
		"sim.protectdesc" => L("PIN на SIM, 2FA приложения, пароль у оператора.", "SIM PIN, 2FA apps.", "SIM-PIN, 2FA-Apps.", "PIN SIM, apps 2FA.", "PIN SIM, apps 2FA."),
		"sim.tip" => L("Совет", "Tip", "Tipp", "Conseil", "Consejo"),
		"sim.tipdesc" => L("Не привязывай SMS к восстановлению паролей.", "Never use SMS for recovery.", "Nie SMS.", "Jamais SMS.", "Nunca SMS."),

		// Урок 8
		"phish.alert" => L("Фишинг — причина 90% взломов.", "Phishing causes 90% of hacks.", "Phishing: 90% der Hacks.", "Phishing: 90% des piratages.", "Phishing: 90% de hackeos."),
		"phish.what" => L("Что такое фишинг?", "What is phishing?", "Was ist Phishing?", "Qu'est-ce que le phishing?", "¿Qué es phishing?"),
		"phish.whatdesc" => L("Поддельные письма, сайты, звонки.", "Fake emails, sites, calls.", "Gefälschte E-Mails.", "Faux emails, sites.", "Falsos emails, sitios."),
		"phish.signs" => L("Признаки фишинга", "Signs of phishing", "Anzeichen", "Signes", "Señales"),
		"phish.signsdesc" => L("Срочность, ошибки, подозрительный URL.", "Urgency, typos, suspicious URL.", "Dringlichkeit, Tippfehler.", "Urgence, fautes, URL.", "Urgencia, errores, URL."),
		"phish.test" => L("Мини-тест", "Mini-test", "Mini-Test", "Mini-test", "Mini-test"),
		"phish.testq" => L("Какой URL настоящий PayPal?", "Which URL is real PayPal?", "Welche URL ist echt?", "Quelle URL est vraie?", "¿Qué URL es real?"),
		"phish.correct" => L("Правильно! paypal.com — верный.", "Correct! paypal.com is real.", "Richtig! paypal.com.", "Correct! paypal.com.", "¡Correcto! paypal.com."),
		"phish.wrong" => L("Нет! Это фишинг. paypal.com — верный.", "No! Phishing. paypal.com is real.", "Nein! Phishing.", "Non! Phishing.", "¡No! Phishing."),

		// Урок 9
		"coin.alert" => L("Bitcoin — псевдонимный. Monero — анонимный.", "Bitcoin pseudonymous. Monero anonymous.", "Bitcoin pseudonym.", "Bitcoin pseudonyme.", "Bitcoin seudónimo."),
		"coin.what" => L("Bitcoin vs Monero", "Bitcoin vs Monero", "Bitcoin vs Monero", "Bitcoin vs Monero", "Bitcoin vs Monero"),
		"coin.whatdesc" => L("Bitcoin: все транзакции видны. Monero: скрыты.", "Bitcoin: visible. Monero: hidden.", "Bitcoin: sichtbar.", "Bitcoin: visible.", "Bitcoin: visible."),
		"coin.anonymous" => L("Как платить анонимно", "How to pay anonymously", "Anonym bezahlen", "Payer anonymement", "Pagar anónimamente"),
		"coin.anonymousdesc" => L("Monero на бирже → в кошелёк → платить.", "Buy Monero → wallet → pay.", "Monero kaufen → Wallet.", "Acheter Monero → wallet.", "Comprar Monero → wallet."),
		"coin.mixers" => L("Миксеры (тумблеры)", "Mixers (tumblers)", "Mixer (Tumbler)", "Mixeurs (tumblers)", "Mixers (tumblers)"),
		"coin.mixersdesc" => L("Перемешивают твои монеты с чужими.", "Mix your coins with others.", "Mischen deine Coins.", "Mélangent tes pièces.", "Mezclan tus monedas."),
		"coin.tip" => L("Совет", "Tip", "Tipp", "Conseil", "Consejo"),
		"coin.tipdesc" => L("Не храни крипту на биржах.", "Don't store on exchanges.", "Nicht an Börsen.", "Pas sur exchanges.", "No en exchanges."),

		// Урок 10
		"opsec.alert" => L("Финальный урок. Стань невидимкой.", "Final lesson. Become invisible.", "Letzte Lektion.", "Dernière leçon.", "Última lección."),
		"opsec.checklist" => L("Чек-лист OPSEC", "OPSEC Checklist", "OPSEC-Checkliste", "Check-list OPSEC", "Checklist OPSEC"),

		// Инструменты
		"tools.title" => L("🧰 ИНСТРУМЕНТЫ", "🧰 TOOLS", "🧰 WERKZEUGE", "🧰 OUTILS", "🧰 HERRAMIENTAS"),
		"tools.desc" => L("Утилиты для цифровой гигиены.", "Digital hygiene utilities.", "Digitale Hygiene.", "Utilitaires hygiène numérique.", "Utilidades de higiene digital."),
		"tools.generator" => L("Генератор паролей", "Password generator", "Passwort-Generator", "Générateur de mot de passe", "Generador de contraseñas"),
		"tools.length" => L("Длина", "Length", "Länge", "Longueur", "Longitud"),
		"tools.upper" => L("Заглавные (A-Z)", "Uppercase (A-Z)", "Großbuchstaben", "Majuscules", "Mayúsculas"),
		"tools.numbers" => L("Цифры (0-9)", "Numbers (0-9)", "Zahlen", "Chiffres", "Números"),
		"tools.symbols" => L("Символы", "Symbols", "Symbole", "Symboles", "Símbolos"),
		"tools.generate" => L("Сгенерировать", "Generate", "Generieren", "Générer", "Generar"),
		"tools.yourpass" => L("Твой пароль", "Your password", "Dein Passwort", "Ton mot de passe", "Tu contraseña"),
		"tools.strength" => L("Надёжность", "Strength", "Stärke", "Force", "Fortaleza"),

		// Шифратор
		"crypto.title" => L("ШИФРОВАНИЕ СООБЩЕНИЙ", "MESSAGE ENCRYPTION", "NACHRICHTENVERSCHLÜSSELUNG", "CHIFFREMENT DE MESSAGES", "CIFRADO DE MENSAJES"),
		"crypto.alert" => L("Шифрование на сервере.", "Server-side encryption.", "Serverseitige Verschlüsselung.", "Chiffrement côté serveur.", "Cifrado del lado del servidor."),
		"crypto.encrypt" => L("Зашифровать", "Encrypt", "Verschlüsseln", "Chiffrer", "Cifrar"),
		"crypto.decrypt" => L("Расшифровать", "Decrypt", "Entschlüsseln", "Déchiffrer", "Descifrar"),
		"crypto.text" => L("Текст", "Text", "Text", "Texte", "Texto"),
		"crypto.password" => L("Пароль", "Password", "Passwort", "Mot de passe", "Contraseña"),
		"crypto.cipher" => L("Шифротекст", "Ciphertext", "Geheimtext", "Texte chiffré", "Texto cifrado"),
		"crypto.encryptbtn" => L("Зашифровать", "Encrypt", "Verschlüsseln", "Chiffrer", "Cifrar"),
		"crypto.decryptbtn" => L("Расшифровать", "Decrypt", "Entschlüsseln", "Déchiffrer", "Descifrar"),
		"crypto.error" => L("Ошибка", "Error", "Fehler", "Erreur", "Error"),
		"crypto.result" => L("Результат", "Result", "Ergebnis", "Résultat", "Resultado"),

		// Профиль
		"profile.title" => L("Цифровой отпечаток", "Digital fingerprint", "Digitaler Fingerabdruck", "Empreinte numérique", "Huella digital"),
		"profile.ghost" => L("Призрачная сущность", "Ghost entity", "Geist-Entität", "Entité fantôme", "Entidad fantasma"),
		"profile.yes" => L("Да", "Yes", "Ja", "Oui", "Sí"),
		"profile.no" => L("Нет", "No", "Nein", "Non", "No"),
		"profile.price" => L("Цена данных", "Data price", "Datenpreis", "Prix des données", "Precio de datos"),
		"profile.empty" => L("Пройди первый урок.", "Complete the first lesson.", "Absolviere die erste Lektion.", "Termine la première leçon.", "Completa la primera lección."),

		// Проверка утечек
		"checker.title" => L("ПРОВЕРКА УТЕЧЕК", "BREACH CHECKER", "DATENLECK-PRÜFUNG", "VÉRIFICATEUR DE FUITES", "VERIFICADOR DE FUGAS"),
		"checker.desc" => L("Проверь пароль и email.", "Check password and email.", "Prüfe Passwort und E-Mail.", "Vérifie mot de passe et email.", "Verifica contraseña y email."),
		"checker.password" => L("Проверка пароля", "Password check", "Passwort-Prüfung", "Vérification mot de passe", "Verificación de contraseña"),
		"checker.passdesc" => L("Пароль проверяется по хэшу.", "Password checked by hash.", "Passwort per Hash geprüft.", "Mot de passe vérifié par hash.", "Contraseña verificada por hash."),
		"checker.email" => L("Проверка email", "Email check", "E-Mail-Prüfung", "Vérification email", "Verificación de email"),
		"checker.emaildesc" => L("Откроем HaveIBeenPwned.", "Open HaveIBeenPwned.", "Öffne HaveIBeenPwned.", "Ouvre HaveIBeenPwned.", "Abre HaveIBeenPwned."),
		"checker.emailapi" => L("Открывается в новом окне.", "Opens in new window.", "Öffnet in neuem Fenster.", "Nouvelle fenêtre.", "Nueva ventana."),
		"checker.openhibp" => L("Открыть HaveIBeenPwned", "Open HaveIBeenPwned", "HaveIBeenPwned öffnen", "Ouvrir HaveIBeenPwned", "Abrir HaveIBeenPwned"),
		"checker.check" => L("Проверить", "Check", "Prüfen", "Vérifier", "Verificar"),
		"checker.checking" => L("Проверяем...", "Checking...", "Prüfe...", "Vérification...", "Verificando..."),
		"checker.pwned" => L("Пароль скомпрометирован!", "Password compromised!", "Passwort kompromittiert!", "Mot de passe compromis!", "¡Contraseña comprometida!"),
		"checker.safe" => L("Пароль не найден!", "Password not found!", "Passwort nicht gefunden!", "Mot de passe introuvable!", "¡Contraseña no encontrada!"),
		"checker.found" => L("Найден в утечках", "Found in breaches", "In Lecks gefunden", "Trouvé dans", "Encontrado en"),
		"checker.times" => L("раз", "times", "Mal", "fois", "veces"),
		"checker.score" => L("Оценка надёжности", "Security score", "Sicherheitswertung", "Score de sécurité", "Puntuación de seguridad"),
		"checker.recommendations" => L("Рекомендации", "Recommendations", "Empfehlungen", "Recommandations", "Recomendaciones"),
		"checker.goto" => L("Создать надёжный пароль", "Generate strong password", "Sicheres Passwort erstellen", "Générer un mot de passe fort", "Generar contraseña segura"),

		// Друзья
		"friends.title" => L("Друзья", "Friends", "Freunde", "Amis", "Amigos"),
		"friends.empty" => L("Список друзей пуст", "Friends list is empty", "Freundesliste ist leer", "Liste d'amis vide", "Lista de amigos vacía"),
		"friends.requests" => L("Заявки в друзья", "Friend requests", "Freundschaftsanfragen", "Demandes d'amis", "Solicitudes de amistad"),
		"friends.add" => L("Добавить в друзья", "Add friend", "Freund hinzufügen", "Ajouter un ami", "Agregar amigo"),
		"friends.sent" => L("Заявка отправлена", "Request sent", "Anfrage gesendet", "Demande envoyée", "Solicitud enviada"),
		"friends.remove" => L("Удалить из друзей", "Remove friend", "Freund entfernen", "Supprimer l'ami", "Eliminar amigo"),

		// Уроки (страница)
		"lessons.title" => L("Уроки", "Lessons", "Lektionen", "Leçons", "Lecciones"),
		"lessons.subtitle" => L("Путь к анонимности", "Path to anonymity", "Weg zur Anonymität", "Chemin vers l'anonymat", "Camino al anonimato"),
		"lessons.completed" => L("ПРОЙДЕН", "DONE", "ABGESCHLOSSEN", "TERMINÉ", "COMPLETADO"),
		"lessons.locked" => L("ЗАКРЫТ", "LOCKED", "GESPERRT", "VERROUILLÉ", "BLOQUEADO"),
		"lessons.order" => L("УРОК", "LESSON", "LEKTION", "LEÇON", "LECCIÓN"),

		// Админ-панель
		"admin.title" => L("Админ-панель", "Admin Panel", "Admin-Panel", "Panneau Admin", "Panel de Admin"),
		"admin.superadmin" => L("Режим: Главный администратор", "Mode: Chief Admin", "Modus: Hauptadmin", "Mode: Admin en chef", "Modo: Admin Jefe"),
		"admin.admin" => L("Режим: Администратор", "Mode: Admin", "Modus: Admin", "Mode: Admin", "Modo: Admin"),
		"admin.stats" => L("Статистика", "Stats", "Statistik", "Statistiques", "Estadísticas"),
		"admin.users" => L("Пользователи", "Users", "Benutzer", "Utilisateurs", "Usuarios"),
		"admin.news" => L("Новости", "News", "News", "Actualités", "Noticias"),
		"admin.appeals" => L("Апелляции", "Appeals", "Einsprüche", "Appels", "Apelaciones"),
		"admin.logs" => L("Логи", "Logs", "Protokolle", "Journaux", "Registros"),
		"admin.search" => L("Поиск по имени...", "Search by name...", "Nach Name suchen...", "Rechercher par nom...", "Buscar por nombre..."),
		"admin.addnews" => L("+ Добавить новость", "+ Add news", "+ News hinzufügen", "+ Ajouter news", "+ Agregar noticia"),
		"admin.chief" => L("Гл. админ", "Chief", "Hauptadmin", "Chef", "Jefe"),
		"admin.adminrole" => L("Админ", "Admin", "Admin", "Admin", "Admin"),
		"admin.user" => L("Пользователь", "User", "Benutzer", "Utilisateur", "Usuario"),
		"admin.banned" => L("Забанен", "Banned", "Gesperrt", "Banni", "Bloqueado"),
		"admin.active" => L("Активен", "Active", "Aktiv", "Actif", "Activo"),

		// Бан
		"banned.title" => L("Доступ заблокирован", "Access blocked", "Zugang gesperrt", "Accès bloqué", "Acceso bloqueado"),
		"banned.reason" => L("Причина бана", "Ban reason", "Sperrgrund", "Raison du ban", "Razón del ban"),
		"banned.admin" => L("Администратор", "Administrator", "Administrator", "Administrateur", "Administrador"),
		"banned.date" => L("Дата бана", "Ban date", "Sperrdatum", "Date du ban", "Fecha del ban"),
		"banned.appeal" => L("Подать апелляцию", "Submit appeal", "Einspruch einlegen", "Faire appel", "Apelar"),

		// Апелляция
		"appeal.title" => L("Подача апелляции", "Submit appeal", "Einspruch einlegen", "Faire appel", "Apelar"),
		"appeal.text" => L("Текст апелляции", "Appeal text", "Einspruchstext", "Texte de l'appel", "Texto de apelación"),
		"appeal.describe" => L("Опишите ситуацию", "Describe the situation", "Situation beschreiben", "Décrivez la situation", "Describe la situación"),
		"appeal.send" => L("Отправить", "Send", "Senden", "Envoyer", "Enviar"),
		"appeal.sent" => L("Апелляция отправлена!", "Appeal sent!", "Einspruch gesendet!", "Appel envoyé!", "¡Apelación enviada!"),
		"appeal.cancel" => L("Отмена", "Cancel", "Abbrechen", "Annuler", "Cancelar"),

		// Профиль (новые)
		"profile.notfound" => L("Профиль не найден", "Profile not found", "Profil nicht gefunden", "Profil introuvable", "Perfil no encontrado"),
		"profile.home" => L("На главную", "Home", "Startseite", "Accueil", "Inicio"),
		"profile.edit" => L("Редактировать профиль", "Edit profile", "Profil bearbeiten", "Modifier le profil", "Editar perfil"),
		"profile.save" => L("Сохранить", "Save", "Speichern", "Enregistrer", "Guardar"),
		"profile.bio" => L("О себе", "About", "Über mich", "À propos", "Sobre mí"),
		"profile.banner" => L("URL баннера", "Banner URL", "Banner-URL", "URL bannière", "URL banner"),
		"profile.avatar" => L("URL аватара", "Avatar URL", "Avatar-URL", "URL avatar", "URL avatar"),
		"profile.displayname" => L("Отображаемое имя", "Display name", "Anzeigename", "Nom affiché", "Nombre mostrado"),
		"profile.achievements" => L("Достижения", "Achievements", "Erfolge", "Réalisations", "Logros"),
		"profile.comments" => L("Комментарии", "Comments", "Kommentare", "Commentaires", "Comentarios"),
		"profile.writecomment" => L("Написать комментарий...", "Write a comment...", "Kommentar schreiben...", "Écrire un commentaire...", "Escribir un comentario..."),
		"profile.nocomments" => L("Пока нет комментариев", "No comments yet", "Noch keine Kommentare", "Pas encore de commentaires", "Sin comentarios aún"),
		"profile.unpin" => L("Убрать значок", "Remove badge", "Abzeichen entfernen", "Retirer le badge", "Quitar insignia"),

		_ => key
	};

	private string L(string ru, string en, string de, string fr, string es) => CurrentLang switch
	{
		"ru" => ru,
		"en" => en,
		"de" => de,
		"fr" => fr,
		"es" => es,
		_ => ru
	};
}