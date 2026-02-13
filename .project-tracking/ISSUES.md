# SyncTrip - Issues & Retours Fonctionnels

**Date** : 13 Fevrier 2026
**Source** : Tests fonctionnels utilisateur
**Priorite globale** : EN COURS DE RESOLUTION — 9/9 issues traitees

---

## Issue 1 : Navigation et structure de l'application
**Priorite** : CRITIQUE
**Statut** : RESOLU (partiel)

**Problemes** :
- ~~Pas de bouton retour dans les pages (convoi detail, cockpit, etc.)~~
- ~~Pas de menu/barre de navigation globale~~
- Pas de page d'accueil / dashboard (non traite — fonctionnel sans)

**Solution appliquee** :
- [x] Barre de navigation avec bouton retour et titre dans MainWindow
- [x] NavigationService : CanGoBack, PageTitle, GoBackCommand, mapping routes → titres
- [x] Header visible uniquement sur les sous-pages (pas login/main)

---

## Issue 2 : Profil — Donnees non sauvegardees + suppression compte
**Priorite** : HAUTE
**Statut** : RESOLU

**Solution appliquee** :
- [x] Sauvegarde profil : reload force apres succes (deja present, flux correct)
- [x] Backend : DeleteUserAccountCommand + Handler + DeleteAsync sur IUserRepository
- [x] API : DELETE /api/users/me sur UsersController
- [x] Mobile : IUserService.DeleteAccountAsync + UserService
- [x] ProfileViewModel : DeleteAccountCommand avec dialog confirmation + logout
- [x] ProfileView : bouton "Supprimer mon compte" en rouge
- [x] Tests : 3 tests (success, not found, error propagation)

---

## Issue 3 : Chargement automatique des donnees
**Priorite** : HAUTE
**Statut** : RESOLU

**Solution appliquee** :
- [x] MainView.axaml.cs : SelectionChanged sur TabControl
- [x] Appel LoadProfileCommand / LoadVehiclesCommand / LoadConvoysCommand au changement d'onglet
- [x] Flag _initialized pour eviter double-chargement au demarrage

---

## Issue 4 : Page Convoi — Informations manquantes
**Priorite** : HAUTE
**Statut** : RESOLU

**Solution appliquee** :
- [x] Encart "Voyage en cours" avec destination et bouton cockpit
- [x] ActiveTripDestination extrait des waypoints (Type==3)
- [x] Bouton "Terminer le voyage" integre dans l'encart

---

## Issue 5 : Nouveau voyage — Interface de destination inutilisable
**Priorite** : CRITIQUE
**Statut** : RESOLU

**Solution appliquee** :
- [x] Recherche d'adresse avec autocompletion (Nominatim)
- [x] Itineraire affiche sur la carte (Mapsui.Nts, GeometryFeature LineString)
- [x] Distance et duree affiches dans l'overlay cockpit
- [x] Choix profil de route (Rapide / Panoramique) via RadioButton
- [x] NavigationApiService.CalculateTripRouteAsync
- [x] CockpitViewModel : RouteGeometry, DistanceText, DurationText, event RouteLoaded

---

## Issue 6 : Geolocalisation — GPS prioritaire, pas IP
**Priorite** : CRITIQUE
**Statut** : AMELIORE (cache 30s au lieu d'infini)

**Solution appliquee** :
- [x] DesktopLocationService : cache TTL 30s au lieu d'infini
- [x] Permet de suivre les changements d'IP en deplacement
- [ ] GPS natif Windows necessiterait changement TFM (risque compatibilite Avalonia)

---

## Issue 7 : Propositions d'arret — Interface popup
**Priorite** : MOYENNE
**Statut** : RESOLU

**Solution appliquee** :
- [x] Panneau overlay voting sur le cockpit (toggle via bouton "Vote")
- [x] VotingViewModel integre comme enfant de CockpitViewModel
- [x] Vote OUI/NON directement dans l'overlay
- [x] Proposition d'arret dans l'overlay (type + nom lieu)

---

## Issue 8 : Chat — Overlay sur la carte
**Priorite** : MOYENNE
**Statut** : RESOLU

**Solution appliquee** :
- [x] Panneau overlay chat sur le cockpit (toggle via bouton "Chat")
- [x] ChatViewModel integre comme enfant de CockpitViewModel
- [x] Messages temps reel + envoi directement dans l'overlay
- [x] Barre de saisie en bas du panneau

---

## Issue 9 : Formulaires — Erreurs de soumission
**Priorite** : HAUTE
**Statut** : RESOLU

**Solution appliquee** :
- [x] ConvoyService.DissolveConvoyAsync : POST → DELETE (verbe HTTP correct)
- [x] Chargement auto des donnees corrige (Issue 3) evite les pages vides

---

## Resume

| Issue | Priorite | Statut |
|-------|----------|--------|
| 1. Navigation | CRITIQUE | RESOLU (partiel — pas de dashboard) |
| 2. Profil + suppression | HAUTE | RESOLU |
| 3. Chargement auto | HAUTE | RESOLU |
| 4. Infos convoi | HAUTE | RESOLU |
| 5. Destination + route | CRITIQUE | RESOLU |
| 6. GPS natif | CRITIQUE | AMELIORE (cache 30s) |
| 7. Vote overlay | MOYENNE | RESOLU |
| 8. Chat overlay | MOYENNE | RESOLU |
| 9. Formulaires HTTP | HAUTE | RESOLU |
