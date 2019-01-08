using UnityEngine;
namespace easy_timer
{
    public struct TimerReturnInfo
    {
        public bool finished;
        public float canonicalVal;
        public float residue; //left over time if we hit the end. 
    }

    [System.Serializable]
    public class Timer
    {
        [HideInInspector]
        public float value;
        public float period;

        public Timer(float period)
        {
            this.period = period;
            this.value = 0.0f;
        }

        public void initTimer(float period)
        {
            this.period = period;
            this.value = 0.0f;
        }


        public bool isOn()
        {
            bool result = this.value >= 0;
            return result;
        }

        public void turnTimerOn()
        {
            this.value = 0;
        }

        public void turnTimerOff()
        {
            this.value = -1;
        }

        public float getTimerValue01()
        {
            float value = this.value / this.period;
            return value;
        }


        public TimerReturnInfo updateTimer(float dt)
        {
            TimerReturnInfo returnInfo = new TimerReturnInfo();

            if (this.value >= 0.0f)
            {
                this.value += dt;
                if (this.period == 0)
                {
                    //NOTE: Set the period if it hasn't been set
                    float defaultPeriod = 1;
                    this.period = defaultPeriod;
                }

                if ((this.value / this.period) >= 1.0f)
                {
                    //TODO: Can a % mod sign do this with floats
                    float minusVal = (this.value - this.period);
                    float lots = minusVal / this.period;
                    returnInfo.residue = minusVal - (lots * this.period);

                    if (!(returnInfo.residue >= 0.0f && returnInfo.residue <= this.period))
                    {
                        //Debug.Log("%f\n", (returnInfo.residue / this.period));
                    }
                    this.value = -1; //turn timer off
                    returnInfo.canonicalVal = 1; //anyone using this value afterwards wants to know that it finished
                    returnInfo.finished = true;
                }
                else
                {
                    returnInfo.canonicalVal = this.getTimerValue01();
                }
            }
            return returnInfo;
        }
    }
}
