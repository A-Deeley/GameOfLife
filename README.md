# Conway's Game of Life

## Description

Reproduction du jeu de la vie de Conway

## Installation
Télécharger l'installateur ici: [Latest Release](https://github.com/kwidz/AndrewSoloProject/releases/tag/v1.0-release)

1) Lancez l'installateur.
2) Cliquez sur `Installer`.
![install](assets/images/installation/1.png)
3) Une fois l'installation terminée, cliquez sur `Terminer`
![done](assets/images/installation/2.png)


## Usage
Lancement:
![launch](assets/images/utilisation/1.png)

Vous pouvez définir une taille custom pour la grille. Voici un exemple d'une grille 15x15:
![customsize](assets/images/utilisation/2.png)

Si les tailles sont correcte, vous pourrez cliquer sur `Create Canvas`.
Ceci créera un canvas, comme ceci:
![canvas](assets/images/utilisation/3.png)

### Dans l'interface:
#### Le Canvas
Dans la grille vous pouvez cliquer sur un carré pour soit la rendre:
- Vivante: ![vivante](assets/images/utilisation/5.png)
- Morte: ![morte](assets/images/utilisation/6.png)

#### Les boutons pour générer les formes
Les 4 premiers boutons sous l'entête `Générer une forme` permettent de charger dans
la grille une forme pré-définie, ainsi qu'une 'forme' aléatoire. Chaque forme à une taille
minimum, sinon quoi le bouton sera grisé puisqu'elle ne rentre pas dans la grille.

##### Forme 1 (Glider)
**Taille**: 3x3
**Forme**: ![glider](assets/images/utilisation/7.png)
##### Forme 2 (Blinker)
**Taille**: 3x3
**Forme**: ![blinker](assets/images/utilisation/8.png)
##### Forme 3 (Gun)
**Taille**: 33x21
**Forme**: ![gun](assets/images/utilisation/9.png)
##### Aléatoire
**Taille**: Aucune
**Forme**: Créee un canvas aléatoire. Voici un exemple:
![aleatoire](assets/images/utilisation/10.png)

#### Charger une forme
Sélectionnez un fichier de type .gol pour la charger dans la grille.
La grille se redimensionnera pour accommoder la taille de la forme.
#### Enregistrer une forme
Donnez un nom a votre fichier pour l'enregistrer!
#### Contrôles d'itérations
Vous pouvez entrer le nombre d'itérations souhaité ou cocher la case `Infini?`.
Le bouton `Démarrer` permet de commencer la simulation, et fait aussi apparaitre
les boutons `Continuer` et `Pause`. 
![pause](assets/images/utilisation/11.png)
Quand on fait `Pause`, le bouton `Itérer` apparait.
![iterate](assets/images/utilisation/12.png)
En dessous vous voyez le numéro de la dernière itération.
#### Contrôle de vitesse
Vous pouvez ralentir (1s par itération) ou accélérer (0.05s par itération) la simulation.
![vitesse](assets/images/utilisation/13.png)

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
