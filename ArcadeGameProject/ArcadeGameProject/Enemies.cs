using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ArcadeGameProject
{
    class Enemies
    {
        //alle, tot nu toe, variabelen voor enemies
        // hoe deze class nu wordt gebruikt is als een extra opslag voor variabelen voor elke enemy op het beeld, dus er wordt voor elke enemy een aparte class met de zelfde naam en variabelenen mogelijkheden gemaakt, alleen de nummers ahter de variabelen kunnen verschillen.
        public Rectangle rectangle;
        public bool ToLeft;
        public bool ToRight;
        public Enemytype enemyType;
        public int InWall;
        public int OutWall;
        public int score;
        public side WhichSide;
        //public int HitTime;
        
    }
}
//enums zijn een soort van status mogelijkheid die gebruikt kan worden in if statements om te kijken of iets voldoet of niet, heel handig
public enum Enemytype {Enemy1, Enemy2 }
public enum side {left, right}