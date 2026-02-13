# SyncTrip - Issues & Retours Fonctionnels

**Date** : 13 Fevrier 2026
**Source** : Tests fonctionnels utilisateur
**Priorite globale** : CRITIQUE — Fonctionnalites manquantes bloquantes

---

## Issue 1 : Navigation et structure de l'application
**Priorite** : CRITIQUE
**Statut** : A FAIRE

**Problemes** :
- Pas de bouton retour dans les pages (convoi detail, cockpit, etc.)
- Pas de menu/barre de navigation globale
- Pas de page d'accueil / dashboard

**Solution attendue** :
- [ ] Ajouter une barre de navigation avec bouton retour sur chaque page
- [ ] Creer une page d'accueil (Dashboard) avec :
  - Nom de l'utilisateur connecte
  - Bouton de deconnexion
  - Convoi en cours (si existant) avec acces direct
  - Bouton "Nouveau convoi" ou "Rejoindre un convoi"
- [ ] Navigation fluide entre les sections

---

## Issue 2 : Profil — Donnees non sauvegardees + suppression compte
**Priorite** : HAUTE
**Statut** : A FAIRE

**Problemes** :
- Les modifications du profil ne sont pas prises en compte (donnees erronees apres modification)
- Pas de moyen de supprimer son compte

**Solution attendue** :
- [ ] Debugger la sauvegarde du profil (verifier requete PUT /api/users/me)
- [ ] Ajouter un bouton "Supprimer mon compte" avec confirmation
- [ ] Implementer l'endpoint backend si inexistant

---

## Issue 3 : Chargement automatique des donnees
**Priorite** : HAUTE
**Statut** : A FAIRE

**Problemes** :
- Les pages Convoi et Vehicules ne chargent pas les donnees au demarrage
- L'utilisateur doit cliquer manuellement sur "Rafraichir" pour voir le contenu

**Solution attendue** :
- [ ] Charger les donnees automatiquement a l'apparition de chaque page/vue
- [ ] Verifier OnAttachedToVisualTree ou OnNavigatedTo pour chaque vue

---

## Issue 4 : Page Convoi — Informations manquantes
**Priorite** : HAUTE
**Statut** : A FAIRE

**Problemes** :
- Pas d'affichage de la destination en cours
- Pas d'encart avec les membres du convoi visible directement
- Pas d'acces direct a la carte

**Solution attendue** :
- [ ] Afficher la destination en cours (ou "Pas de voyage actif")
- [ ] Afficher la liste des membres dans un encart visible
- [ ] Bouton d'acces direct a la carte/cockpit

---

## Issue 5 : Nouveau voyage — Interface de destination inutilisable
**Priorite** : CRITIQUE
**Statut** : BACKEND FAIT (geocoding Nominatim + routing OSRM + NavigationController) — Mobile A FAIRE

**Problemes** :
- Saisie manuelle de latitude/longitude = inutilisable pour un utilisateur normal
- Pas de recherche d'adresse/lieu (type Google Maps / Waze)
- Pas d'itineraire affiche (seulement point de depart et arrivee)
- Pas de choix du profil de route (autoroute vs petites routes) alors que prevu dans la doc

**Solution attendue** :
- [ ] Integrer un service de geocoding (recherche d'adresse → coordonnees)
  - Option : Nominatim (OpenStreetMap, gratuit, pas de cle API)
  - Ou : API de geocoding tierce
- [ ] Remplacer les champs lat/lon par un champ de recherche d'adresse avec autocompletion
- [ ] Integrer un service de routing pour afficher l'itineraire sur la carte
  - Option : OSRM (Open Source Routing Machine, gratuit)
  - Ou : OpenRouteService
- [ ] Afficher le trace de l'itineraire sur la carte Mapsui (polyline)
- [ ] Ajouter le choix du profil de route (Autoroute rapide / Routes panoramiques)
- [ ] Calculer et afficher la distance et la duree estimee

---

## Issue 6 : Geolocalisation — GPS prioritaire, pas IP
**Priorite** : CRITIQUE
**Statut** : A FAIRE

**Problemes** :
- La position est calculee par geolocalisation IP (ip-api.com)
- En deplacement (5G/4G), l'IP change constamment → position fausse
- Pour une app de navigation en convoi, le GPS de l'appareil est indispensable

**Solution attendue** :
- [ ] Utiliser le GPS natif de l'appareil en priorite
  - Desktop Windows : Windows.Devices.Geolocation (WinRT API)
  - Android/iOS : APIs natives (futur)
- [ ] Fallback sur geolocalisation IP uniquement si GPS indisponible (desktop sans GPS)
- [ ] Frequence de mise a jour GPS : 5s en mouvement

---

## Issue 7 : Propositions d'arret — Interface popup
**Priorite** : MOYENNE
**Statut** : A FAIRE

**Problemes** :
- L'interface de proposition d'arret n'est pas accessible facilement
- Devrait etre une popup/overlay sur la carte, pas une page separee

**Solution attendue** :
- [ ] Proposer un arret via popup/overlay sur la carte du cockpit
- [ ] Notification visuelle quand une proposition est active
- [ ] Vote OUI/NON directement dans l'overlay

---

## Issue 8 : Chat — Overlay sur la carte
**Priorite** : MOYENNE
**Statut** : A FAIRE

**Problemes** :
- Le chat est sur une page separee
- Devrait etre accessible en overlay sur la carte du cockpit

**Solution attendue** :
- [ ] Ajouter un overlay/panneau lateral pour le chat sur le cockpit
- [ ] Bouton toggle pour ouvrir/fermer le chat
- [ ] Notifications de nouveaux messages

---

## Issue 9 : Formulaires — Erreurs de soumission
**Priorite** : HAUTE
**Statut** : A FAIRE

**Problemes** :
- Les formulaires affichent des erreurs alors que l'action reussit
- Necessitent un rafraichissement pour voir le resultat
- Problemes de deserialisation des reponses API

**Solution attendue** :
- [ ] Auditer tous les services HTTP (ApiService, UserService, VehicleService, ConvoyService, TripService)
- [ ] Verifier les URLs, verbes HTTP, et formats de reponse
- [ ] Tester chaque formulaire de bout en bout
- [ ] Afficher un message de succes apres chaque action reussie

---

## Ordre de priorite recommande

1. **Issue 5** : Geocoding + itineraire (fonctionnalite coeur manquante)
2. **Issue 6** : GPS natif (fonctionnalite coeur manquante)
3. **Issue 1** : Navigation + Dashboard (UX bloquante)
4. **Issue 3** : Chargement auto des donnees (UX bloquante)
5. **Issue 9** : Formulaires (bugs)
6. **Issue 2** : Profil (bugs + feature)
7. **Issue 4** : Page Convoi (UX)
8. **Issue 7** : Popup arret (UX)
9. **Issue 8** : Chat overlay (UX)
