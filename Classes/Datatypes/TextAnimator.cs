using System;
using Godot;

namespace Novel
{
    public class TextAnimator
    {
        private Label textToAnimate;
        private string textToAnimateString;
        private double animationTime;
        private double animationTimer = 0.0f;

        public TextAnimator(Label aTextToAnimate, string aTextToAnimateString, float aAnimationTime)
        {
            textToAnimate = aTextToAnimate;
            textToAnimateString = aTextToAnimateString;
            animationTime = aAnimationTime;
        }


        public void Update(double delta)
        {
            animationTimer += delta;
            if (animationTimer > animationTime)
            {
                animationTimer = animationTime;
            }

            int textLength = (int) ( animationTimer / animationTime  * textToAnimateString.Length);
            textToAnimate.Text = textToAnimateString.Substring(0, textLength);
        }

        public bool IsAnimationFinished()
        {
            return animationTimer >= animationTime;
        }

        public void SkipAnimation()
        {
            animationTimer = animationTime;
        }
    }
}