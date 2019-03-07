using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Must include Spriter Namespace
using SpriterDotNetUnity;

enum animationMode
{
    lookUp,
    lookDown,
    move,
    straightJump,
    rollingJump,
}

public class EmberAnimationCycle : MonoBehaviour
{
    /* 
     * Instead of putting the script on Spriter's automatically generated prefab, which will cause it to be unset every
     * time Evan tweaks the damn animations, we put the control scripts on a seperate gameobject which becomes a parent
     * of the Spriter prefab and take a reference to the Spriter object so we can manipulate it.
    */
    public GameObject Ember;

    // This is the actual thing we use to animate the character.
    UnityAnimator anim;
    
    void Start()
    {
    }
    
    bool slash = false;
    animationMode mode = animationMode.lookUp;

    void Update()
    {
        if (Ember == null)
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Ember") { Ember = child.gameObject; break;}
            }
        }

        if (anim == null)
        {
            anim = Ember.GetComponent<SpriterDotNetBehaviour>().Animator;
            //This event is fired whenever an animation ends.
            anim.AnimationFinished += animationTransitions;
        }

        if (Input.anyKeyDown)
        {
            //Advance the animation.
            if (anim.CurrentAnimation.Name == "Statue") anim.Play("Statue to Idle");
            else if (anim.CurrentAnimation.Name == "Idle")
            {
                switch (mode)
                {
                    case animationMode.move:
                        anim.Play("Idle to Walk");
                        mode = animationMode.straightJump;
                        break;
                    case animationMode.straightJump:
                        anim.Play("Begin Straight Jump");
                        mode = animationMode.rollingJump;
                        break;
                    case animationMode.rollingJump:
                        anim.Play("Begin Rolling Jump");
                        mode = animationMode.lookUp;
                        break;
                    case animationMode.lookUp:
                        if (!slash)
                        {
                            anim.Play("Grounded Forward Slash");
                            slash = true;
                        }
                        else
                        {
                            slash = false;
                            anim.Play("Idle to Lookup");
                            mode = animationMode.lookDown;
                        }
                        break;
                    case animationMode.lookDown:
                        anim.Play("Idle to Crouch");
                        mode = animationMode.move;
                        break;
                }
            }
            else if (anim.CurrentAnimation.Name == "Walk") anim.Play("Run");
            else if (anim.CurrentAnimation.Name == "Run") anim.Play("Idle");
            else if (anim.CurrentAnimation.Name == "Straight Jump Rising")
            {
                if (!slash)
                {
                    anim.Play("Airborn Upward Slash");
                    slash = true;
                }
                else
                {
                    anim.Play("Straight Jump Crest");
                    slash = false;
                }
            }
            else if (anim.CurrentAnimation.Name == "Straight Jump Falling"
                || anim.CurrentAnimation.Name == "Rolling Jump")
            {
                if (!slash)
                {
                    if (anim.CurrentAnimation.Name == "Straight Jump Falling") anim.Play("Airborn Forward Slash");
                    else anim.Play("Airborn Downward Slash");
                    slash = true;
                }
                else
                {
                    anim.Play("Straight Jump Landing");
                    slash = false;
                }
            }
            else if (anim.CurrentAnimation.Name == "Crouch") anim.Play("Crouch to Idle");
            else if (anim.CurrentAnimation.Name == "Lookup")
            {
                if (!slash)
                {
                    anim.Play("Grounded Upward Slash");
                    slash = true;
                }
                else
                {
                    anim.Play("Lookup to Idle");
                    slash = false;
                }
            }
        }
    }

    void animationTransitions(string name)
    {
        //We check to see if it's one of our transition animations and if so advance to the animation
        // we are transitioning to.
        if (name == "Statue to Idle") anim.Play("Idle");
        else if (name == "Idle to Walk") anim.Play("Walk");
        else if (name == "Begin Straight Jump") anim.Play("Straight Jump Rising");
        else if (name == "Straight Jump Crest") anim.Play("Straight Jump Falling");
        else if (name == "Begin Rolling Jump") anim.Play("Rolling Jump");
        else if (name == "Straight Jump Landing") anim.Play("Idle");
        else if (name == "Idle to Crouch") anim.Play("Crouch");
        else if (name == "Crouch to Idle") anim.Play("Idle");
        else if (name == "Idle to Lookup") anim.Play("Lookup");
        else if (name == "Lookup to Idle") anim.Play("Idle");
        else if (name == "Grounded Forward Slash") anim.Play("Idle");
        else if (name == "Grounded Upward Slash") anim.Play("Lookup");
        else if (name == "Airborn Upward Slash") anim.Play("Straight Jump Rising");
        else if (name == "Airborn Forward Slash") anim.Play("Straight Jump Falling");
        else if (name == "Airborn Downward Slash") anim.Play("Rolling Jump");
    }
}
