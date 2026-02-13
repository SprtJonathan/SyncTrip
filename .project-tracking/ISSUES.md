# SyncTrip - Issues & Retours Fonctionnels

---

## VAGUE 1 — Tests du 13 Fevrier 2026 (Session 1)

**Statut** : 9/9 issues traitees (7 RESOLU, 1 partiel, 1 AMELIORE)

<details>
<summary>Voir le detail des issues resolues</summary>

| Issue | Priorite | Statut |
|-------|----------|--------|
| 1. Navigation (retour + header) | CRITIQUE | RESOLU (partiel — pas de dashboard) |
| 2. Profil + suppression compte | HAUTE | RESOLU |
| 3. Chargement auto onglets | HAUTE | RESOLU |
| 4. Infos convoi enrichies | HAUTE | RESOLU |
| 5. Destination + itineraire carte | CRITIQUE | RESOLU |
| 6. GPS natif (cache 30s) | CRITIQUE | AMELIORE |
| 7. Vote overlay cockpit | MOYENNE | RESOLU |
| 8. Chat overlay cockpit | MOYENNE | RESOLU |
| 9. Formulaires HTTP (POST→DELETE) | HAUTE | RESOLU |

</details>

---

## VAGUE 2 — Tests du 13 Fevrier 2026 (Session 2)

**Date** : 13 Fevrier 2026
**Source** : Tests fonctionnels utilisateur (lancement Desktop)
**Statut** : A RESOUDRE — 0/10 issues traitees

---

### Issue 10 : Magic Link ne s'envoie pas
**Priorite** : CRITIQUE (bloquant — impossible de se connecter)
**Statut** : A RESOUDRE

**Symptome** :
- L'envoi du magic link echoue avec "Une erreur est survenue"
- Aucun detail sur l'erreur (pas de code, pas de message explicite)
- Impossible de savoir si c'est un probleme SMTP, backend, ou reseau

**Actions requises** :
- [ ] Diagnostiquer la cause racine (SMTP ? Backend down ? URL API incorrecte ? Timeout ?)
- [ ] Verifier que le backend est bien accessible depuis l'app (BaseAddress, port, CORS)
- [ ] Verifier la configuration SMTP (User Secrets, credentials Gmail)
- [ ] Ajouter des messages d'erreur explicites avec codes (voir Issue 15)
- [ ] Ajouter des logs dans MagicLinkViewModel et AuthenticationService

---

### Issue 11 : Pas de page d'accueil / dashboard
**Priorite** : HAUTE
**Statut** : A RESOUDRE

**Symptome** :
- Apres connexion, l'utilisateur arrive directement sur les onglets (Profil/Garage/Convois)
- Pas de vue d'ensemble : pas de resume des convois actifs, voyages en cours, notifications
- L'experience utilisateur manque de contexte a l'arrivee

**Actions requises** :
- [ ] Creer une page Dashboard (HomeView / HomeViewModel)
- [ ] Afficher : convois actifs, dernier voyage, notifications recentes
- [ ] En faire le premier onglet du TabControl (avant Profil)
- [ ] Boutons d'acces rapide : "Rejoindre un convoi", "Creer un convoi"

---

### Issue 12 : Profil — Impossible de charger les informations
**Priorite** : CRITIQUE (bloquant)
**Statut** : A RESOUDRE

**Symptome** :
- Page Profil affiche "Impossible de recuperer les informations du profil"
- Message generique "Une erreur est survenue" sans detail
- Probablement lie a un probleme de connexion API ou d'authentification (JWT expire ? Header manquant ?)

**Actions requises** :
- [ ] Diagnostiquer : verifier que le JWT est bien envoye dans les headers API
- [ ] Verifier l'endpoint GET /api/users/me (accessible ? 401 ? 500 ?)
- [ ] Ajouter des messages d'erreur detailles (voir Issue 15)
- [ ] Ajouter des logs dans ProfileViewModel et UserService

---

### Issue 13 : Garage — Liste des marques ne charge pas
**Priorite** : CRITIQUE (bloquant — impossible d'ajouter un vehicule)
**Statut** : A RESOUDRE

**Symptome** :
- La liste des marques (brands) ne se charge pas dans le formulaire d'ajout vehicule
- Message "Une erreur est survenue" sans detail
- Sans marques, impossible de creer un vehicule → bloque aussi les convois

**Actions requises** :
- [ ] Diagnostiquer : verifier l'endpoint GET /api/brands (accessible ? seed data present ?)
- [ ] Verifier que la migration DB a bien seed les 40 marques
- [ ] Ajouter des messages d'erreur detailles (voir Issue 15)
- [ ] Ajouter des logs dans GarageViewModel et BrandService

---

### Issue 14 : Convois — Impossible a tester (bloque par vehicule)
**Priorite** : HAUTE (bloquee par Issue 13)
**Statut** : A RESOUDRE

**Symptome** :
- La page convoi necessite d'avoir un vehicule pour creer/rejoindre un convoi
- Comme l'ajout de vehicule est bloque (Issue 13), les convois sont inaccessibles

**Actions requises** :
- [ ] Resoudre d'abord Issue 13 (chargement marques)
- [ ] Re-tester la creation et la jonction de convoi
- [ ] Verifier que le vehicule selectionne est bien envoye dans les requetes

---

### Issue 15 : Messages d'erreur generiques — Pas de codes ni de logs
**Priorite** : CRITIQUE (transversal — impacte le debug de toutes les issues)
**Statut** : A RESOUDRE

**Symptome** :
- Toutes les erreurs affichent "Une erreur est survenue" sans aucun detail
- Pas de code d'erreur (ex: ERR_AUTH_001, ERR_PROFILE_002)
- Pas de logs visibles dans la console Desktop
- Impossible de diagnostiquer les problemes sans ouvrir le debugger

**Actions requises** :
- [ ] Implementer un systeme de codes d'erreur structure (par module : AUTH, PROFILE, VEHICLE, CONVOY, TRIP)
- [ ] Afficher des messages utilisateur explicites : "Serveur inaccessible", "Session expiree", "Echec SMTP", etc.
- [ ] Distinguer les erreurs reseau (timeout, DNS) des erreurs metier (401, 404, 422, 500)
- [ ] Ajouter du logging structure dans tous les ViewModels et Services (ILogger<T>)
- [ ] Afficher les logs dans la console Desktop (Serilog ou Microsoft.Extensions.Logging vers Console)
- [ ] En mode debug : afficher le message technique complet (ex.Message) en plus du message utilisateur

---

### Issue 16 : Chargement des donnees globalement casse (connexion BDD/API)
**Priorite** : CRITIQUE (bloquant — cause racine probable des issues 10, 12, 13)
**Statut** : A RESOUDRE

**Symptome** :
- Aucune donnee ne se charge (profil, marques, convois)
- Toutes les pages affichent des erreurs
- Indique un probleme systémique : soit le backend ne tourne pas, soit l'URL API est mauvaise, soit le JWT n'est pas transmis

**Actions requises** :
- [ ] Verifier que Docker (PostgreSQL + API) tourne : `docker compose ps`
- [ ] Verifier que l'API repond : `curl http://localhost:5000/scalar/v1`
- [ ] Verifier le BaseAddress dans ApiService (http://localhost:5000 ?)
- [ ] Verifier que AuthorizationMessageHandler injecte bien le Bearer token
- [ ] Ajouter un health check au demarrage de l'app (ping API → message clair si KO)
- [ ] Ajouter un indicateur de connexion dans le header (vert = connecte, rouge = deconnecte)

---

### Issue 17 : Temps de chargement tres longs
**Priorite** : HAUTE
**Statut** : A RESOUDRE

**Symptome** :
- Les pages mettent tres longtemps a charger (loader visible pendant plusieurs secondes)
- L'interface semble figee pendant les appels API
- Mauvaise experience utilisateur, surtout sur Desktop

**Actions requises** :
- [ ] Profiler les appels API (temps de reponse backend vs temps reseau)
- [ ] Verifier qu'il n'y a pas de requetes sequentielles inutiles (paralleliser si possible)
- [ ] Ajouter du cache local pour les donnees stables (marques, profil utilisateur)
- [ ] Verifier que les appels ne sont pas faits en double (OnAttachedToVisualTree + SelectionChanged)
- [ ] Skeleton screens au lieu de spinners pleins

---

### Issue 18 : Interface Desktop pas adaptee — Priorite au mobile
**Priorite** : HAUTE
**Statut** : A RESOUDRE

**Symptome** :
- L'interface Desktop n'est pas optimale (layout trop large, pas de responsive)
- SyncTrip est une app de convoi vehicule → l'usage principal est MOBILE
- La version Desktop devrait etre secondaire (pour dev/test uniquement)
- Besoin d'un emulateur Android ou iOS pour tester en conditions reelles

**Actions requises** :
- [ ] Activer le platform head Android (`SyncTrip.App.Android`) avec emulateur
- [ ] OU activer iOS si Mac disponible
- [ ] Adapter les vues pour un format mobile (largeur ~400px, touch-friendly)
- [ ] Boutons plus grands, espacement tactile, polices adaptees
- [ ] Tester la navigation au doigt (scroll, swipe, tap)
- [ ] Le Desktop peut rester en mode "fenetre redimensionnable" pour le dev

---

### Issue 19 : Chargement initial des pages (lifecycle)
**Priorite** : MOYENNE
**Statut** : A RESOUDRE

**Symptome** :
- Les pages ne chargent pas toujours leurs donnees correctement au premier affichage
- Probleme potentiel de lifecycle Avalonia (OnAttachedToVisualTree vs DataContext ready)
- Certaines pages peuvent s'afficher vides avant que le ViewModel soit initialise

**Actions requises** :
- [ ] Auditer le lifecycle de chaque page (quand est-ce que LoadXxx est appele ?)
- [ ] S'assurer que le DataContext est set AVANT OnAttachedToVisualTree
- [ ] Ajouter des etats visuels : Loading / Empty / Error / Loaded
- [ ] Tester la navigation aller-retour (back puis re-entree dans une page)

---

## Resume — Vague 2

| # | Issue | Priorite | Statut | Bloque par |
|---|-------|----------|--------|------------|
| 10 | Magic Link echoue | CRITIQUE | A RESOUDRE | — |
| 11 | Pas de dashboard | HAUTE | A RESOUDRE | — |
| 12 | Profil ne charge pas | CRITIQUE | A RESOUDRE | Issue 16 |
| 13 | Marques ne chargent pas | CRITIQUE | A RESOUDRE | Issue 16 |
| 14 | Convois inaccessibles | HAUTE | A RESOUDRE | Issue 13 |
| 15 | Messages d'erreur generiques | CRITIQUE | A RESOUDRE | — |
| 16 | Connexion BDD/API cassee | CRITIQUE | A RESOUDRE | — |
| 17 | Chargement tres lent | HAUTE | A RESOUDRE | — |
| 18 | Desktop → Mobile prioritaire | HAUTE | A RESOUDRE | — |
| 19 | Lifecycle pages (init) | MOYENNE | A RESOUDRE | — |

**Ordre de resolution recommande** :
1. **Issue 16** (connexion API/BDD) — cause racine probable, debloque 12, 13, 14
2. **Issue 15** (messages d'erreur + logs) — indispensable pour debugger efficacement
3. **Issue 10** (magic link) — bloquant pour tout le flux utilisateur
4. **Issue 12 + 13** (profil + marques) — devrait se resoudre avec Issue 16
5. **Issue 14** (convois) — re-tester apres 13
6. **Issue 17** (performance) — optimiser une fois que tout fonctionne
7. **Issue 19** (lifecycle) — stabiliser le chargement
8. **Issue 11** (dashboard) — amelioration UX
9. **Issue 18** (mobile) — cible finale
