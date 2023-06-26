using System;

namespace Settings
{
    public class Enumerators
    {
        public enum NamePrefabAddressable : short
        {
            PlayerGladiator = 0,
            PlayerFighterGladiator = 1,

            SquareLevel = 50,
            ArenaColiseum = 51,

            //NPC
            PassantGladiator = 100,
            FighterGladiator = 101,


            Material = -1,
            Coin = -2,
            MarkerFighter = -3,
            Sword = -4,
            Shield = -5,
            Message = -6,
            CFXRHitARed = -7,
            PointerArrow = -8,

            //UI
            Stick = 200,
            InteractiveMiniGamePopup = 201,
            ArenaPopup = 202,
            ColiseumPopup = 203,
            ArenaPopupLose = 204,
            FoldingScreenArena = 205,
            GymPopup = 206,
            UpdateAmmunitionPopup = 207,
            MessagePopup = 208,
            CharacteristicsFightersPopup = 209,


            InGameArenaPage = 250,
            InGameSquarePage = 251,
        }

        public enum AppState
        {
            Unknown,

            AppStart,
            InGameSquare,
            InGameArena,
        }

        public enum ReceptacleObjectType : short
        {
            Unknown,

            Material = -1,
            Coin = -2,
            Sword = -4,
            Shield = -5,
        }

        public enum InteractiveObjectType
        {
            Crafts,
            ToBuy,
            Exercise
        }

        public enum InteractiveObjectName : ushort
        {
            Unknown = 0,

            WorkbenchSword = 1,
            WorkbenchShield = 2,
            TrainingApparatusBarbell = 3,
            TrainingApparatusMakiwara = 4,
            TrainingApparatusPunchingBag = 5,
        }

        public enum StorekeeperType
        {
            Unknown,

            ToTake,
            ToGive
        }
        public enum WorkbenchShield
        {
            PressDown,
            PressUp
        }

        public enum AnimatorUserType
        {
            Unknown,

            Player,
        }
        public enum SoundType
        {
            Unknown,
        }
        public enum SoundName
        {
            Unknown,
            Coin,
            Forging,
            TransferItemsBetweenWarehouses,
            SuccessfulAttackNPC,
            SuccessfulAttackPlayer,
            Def,
            Audience,
            Square1,
            Square2,
            Fight,
            CraftingZone,
            Stun,
            AddingCharacteristics,
            Dodge
        }

        public enum PlayerAnimations
        {
            //animations of this type are similar for all stickman
            Idle = 0,


            Run = 1,
            Interaction
        }

        public enum NPCAnimations
        {
            //animations of this type are similar for all stickman
            Idle = 0,


            IdleAlley = 1,
            IdleShop,
            Walking,
            DidNotBuy,
            Buy
        }

        public enum FighterAnimations
        {
            Idle,

            Attack,
            Protection,
            Lose,
            Win,
            Reset,
            Damage,
            DodgeLeft,
            DodgeRight,
            BattleStart,
            Stunned
        }

        public enum UpdateType
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        public enum FighterState
        {
            Idle,

            Attack,
            Protection,
            Lose,
            Win,
            Damage,
            Dodge,
            StartBattle,
            Stunned
        }
        
        public enum ClickArenaState
        {
            Attack,
            Protection,
            DodgeRight,
            DodgeLeft
        }

        public enum PlayerStates
        {
            Inaction,
            Movement,
            Storekeeper,
            Interaction
        }

        public enum NPCStates
        {
            Unknown,

            Buy,
            Movement,
            DoNothing,
        }
        public enum NPCArchetype: ushort
        {
            Normal = 0,
            Aggressive = 1,
            Defensive = 2,
        }

        public enum PlaceInterestType
        {
            Unknown,

            SpawnPosPlace,
            AlleyPosPlace,
            AlleyFrontShopPosPlace,
            ShopPosPlace,
            RackPosPlace,
            TutorCamera,
            TutorArrow
        }
        public enum EmotionsType
        {
            Unknown,

            Evil,
        }
        public enum ParticleType
        {
            Red,
            Blue,
            Yellow,
            Stars
        }
        public enum LockLocation
        {
            All,
            Forge,
            Gym
        }
        public enum TutorialStep
        {
            PurchaseOfAnAnvil,
            ShowStock,
            DropMaterial,
            MachineWork,
            TakeSwords,
            ShowRack,
            TakeMoney,
            ShowColosseum,
            GreatLoss,
        }
    }
}