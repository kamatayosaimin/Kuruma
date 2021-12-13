using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OYKT
{

    public enum DriveType
    {
        FrontDrive,
        RearDrive,
        FourWheelDrive
    }

    [Serializable]
    struct AutomaticTransmission
    {
        [SerializeField]
        private float down, up;
        [SerializeField]
        private bool isAT;

        public float Down
        {
            get
            {
                return down;
            }
        }

        public float Up
        {
            get
            {
                return up;
            }
        }

        public bool IsAT
        {
            get
            {
                return isAT;
            }

            set
            {
                isAT = value;
            }
        }
    }

    [Serializable]
    struct KURUMAInput : KURUMAInputGetter
    {
        private float motor, brake, steering;
        [SerializeField]
        private float steeringDamp;

        public float Motor
        {
            get
            {
                return motor;
            }
        }

        public float Brake
        {
            get
            {
                return brake;
            }
        }

        public float Steering
        {
            get
            {
                return steering;
            }
        }

        public void SetInput(float motor, float brake, float steering)
        {
            this.motor = motor;
            this.brake = brake;
            this.steering = GetCurrentSteering(steering);
        }

        float GetCurrentSteering(float steering)
        {
            float damp = steeringDamp * Time.deltaTime;

            return Mathf.Lerp(this.steering, steering, damp);
        }
    }

    [Serializable]
    struct Mission
    {
        [Range(2.5f, 6f)]
        [SerializeField]
        private float finalGear;
        [Range(0f, 6f)]
        [SerializeField]
        private float[] gearRatio;

        public float FinalGear
        {
            get
            {
                return finalGear;
            }
        }

        public float[] GearRatio
        {
            get
            {
                return gearRatio;
            }
        }
    }

    [Serializable]
    struct Powertrein
    {
        [SerializeField]
        private float brake, steering, clutchDistance, clutchDamp;
        [Range(0f, 1f)]
        [SerializeField]
        private float clutchOnDistance;
        [Range(0.5f, 1f)]
        [SerializeField]
        private float brakeFront, brakeRear;
        [SerializeField]
        private DriveType driveType;

        public float BrakeFront
        {
            get
            {
                return brake * brakeFront;
            }
        }

        public float BrakeRear
        {
            get
            {
                return brake * brakeRear;
            }
        }

        public float Steering
        {
            get
            {
                return steering;
            }
        }

        public float ClutchDistance
        {
            get
            {
                return clutchDistance;
            }
        }

        public float ClutchDamp
        {
            get
            {
                return clutchDamp;
            }
        }

        public float ClutchOnDistance
        {
            get
            {
                return clutchOnDistance;
            }
        }

        public DriveType DriveType
        {
            get
            {
                return driveType;
            }
        }
    }

    [Serializable]
    public struct Tire
    {
        [SerializeField]
        private float squealPitchMultipler, smokeEmissionRate, squealDamp;
        [Range(0f, 1f)]
        [SerializeField]
        private float sidewaysSlipRate, trailSmokeTarget;

        public float SquealPitchMultipler
        {
            get
            {
                return squealPitchMultipler;
            }
        }

        public float SmokeEmissionRate
        {
            get
            {
                return smokeEmissionRate;
            }
        }

        public float SquealDamp
        {
            get
            {
                return squealDamp;
            }
        }

        public float SidewaysSlipRate
        {
            get
            {
                return sidewaysSlipRate;
            }
        }

        public float TrailSmokeTarget
        {
            get
            {
                return trailSmokeTarget;
            }
        }
    }

    [Serializable]
    struct TorqueControl
    {
        [Range(0f, 1f)]
        [SerializeField]
        private float frontBrakeRate, rearBrakeRate;

        public float FrontBrakeRate
        {
            get
            {
                return frontBrakeRate;
            }
        }

        public float RearBrakeRate
        {
            get
            {
                return rearBrakeRate;
            }
        }
    }

    [Serializable]
    public class EngineSpec
    {
        [SerializeField]
        private int idleRpm, revLimit;
        [SerializeField]
        private float maxTorqueKgM;
        [SerializeField]
        [RangeCurve(0f, 0f, 1f, 1f)]
        private AnimationCurve torqueRate;

        public int IdleRpm
        {
            get
            {
                return idleRpm;
            }
        }

        public int RevLimit
        {
            get
            {
                return revLimit;
            }
        }

        public float MaxTorqueKgM
        {
            get
            {
                return maxTorqueKgM;
            }
        }

        public AnimationCurve TorqueRate
        {
            get
            {
                return torqueRate;
            }
        }
    }

    public interface KURUMAInputGetter
    {

        float Motor
        {
            get;
        }

        float Brake
        {
            get;
        }

        float Steering
        {
            get;
        }
    }
}
