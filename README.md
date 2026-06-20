**Halloween Runner**
*Subway Surfers Revisité*

Voici notre jeu Subway Surfers revisité.
Ce jeu reprend les codes classiques du célèbre jeu Subway Surfers mais avec un thème thriller.
Le but du jeu est simple : il suffit d'obtenir le score le plus élevé possible et de ramasser le plus de pièces.

Mécaniques du jeu :
  - Le joueur est capable de se déplacer de gauche à droite avec le clavier ou la souris (drag & drop).
  - Il peut également sauter et glisser au sol pour éviter les obstacles. Il ramasse des pièces en courant.
  - Le joueur a au total 2 vies et s'il touche deux obstacles il perd le jeu. Sinon, le jeu est infini, le but est d'avoir le score le plus éléeé possible.
  - Système d'invincibilité temporaire : après avoir touché un obstacle, le joueur bénéficie d'une courte période d'invincibilité de 1.2 secondes pendant laquelle il ne peut pas perdre de vie supplémentaire. Cela évite de perdre plusieurs vies sur un même obstacle.

Remarques particulières (limitations, bugs connus, voies d'amélioration): 
  - Pour le moment, notre chemin infini est limité à seulement 6 variations de chemin, ce qui rend le jeu assez redondant après une certaine durée.
  - Nous pensons qu'il serait intéressant d'ajouter des sessions individuelles pour chaque utilisateur afin de récupérer leur scores et créer un classement général.
  - Il serait également interéssant de mettre en place un magasin sur notre jeu afin de pouvoir acheter des items pour notre joueur.
  - Ajouter des items spéciaux à récupérer pendant la course du joueur peut également améliorer notre gameplay.


Répartition de la production :
      Alkim GOR -> Gestion et intégration backend des animations du personnage principal, du sound design, de la création frontend des écrans du jeu.
      Anchana FATIMA RAJAN -> Gestion backend des écrans du jeu, du sound design et création du système pièces et score, suivi des branches.
      Marie-Emilienne GNAMIEN -> Gestion backend de la fonctionnaité "chemin infini" et création des mouvements du personnage (clavier et souris).
      Nikola KALETKA -> Gestion backend des obstacles du jeu, des vies restantes du joueur et du comportement de la fantôme. 
      
      
Assets utilisés :
Le chemin infini est composé d'un plan simple créé depuis Unity associé à des assets récupérés sur internet pour les obstacles.

Décor et obstacles : https://assetstore.unity.com/packages/3d/environments/fantasy/halloween-cemetery-set-19125
Pièces : https://assetstore.unity.com/packages/3d/props/metallic-coin-free-trial-241879
Personnage principal : https://www.mixamo.com/#/?page=1&query=michelle&type=Character
Fantôme : https://www.turbosquid.com/fr/3d-models/scary-ghost-3d-model-2437198
Ciel Deep Dusk : https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-free-10-sky-skybox-set-146014

Voici le paramétrage mouvement à mettre en place sur votre poste afin de voir correctement les animations : 
Project -> Assets -> Ch03.fbx -> Animation -> Loop Time (oui), root transform (y) (oui), root transform (x) (oui), root transform (z) (non).
