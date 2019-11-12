/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using UnityEngine;

namespace TouchScript.Examples.RawInput
{


    /// <exclude />
    public class Spawner : MonoBehaviour
    {
        SpriteRenderer m_SpriteRenderer;
        float blue_comp = 1;
        float green_comp = 0;
        public float food_process_rate;

        bool penetratio = false;
        bool transporting = false;
        bool working = false;

        Vector3 w_l1_pos;

        Collider2D [] work_squares;

        Transform transporter;

        void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_SpriteRenderer.color = new Color(1, green_comp, blue_comp);

            work_squares = GameObject.FindGameObjectWithTag("work_row_middle").GetComponentsInChildren<Collider2D>();

          //  Debug.Log(work_squares[4].transform.position.y);
        }

        private void OnEnable()
        {
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersPressed += pointersPressedHandler;
            }
        }

        private void OnDisable()
        {
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersPressed -= pointersPressedHandler;
            }
        }

        private void pointersPressedHandler(object sender, PointerEventArgs e)
        {
            foreach (var pointer in e.Pointers)
            {


                //Debug.Log(pointer.Position.x);

                if (!transporting & !working)
                {
                    if(penetratio)
                    {
                        transporting = true;

                        transform.parent = transporter;

                    }

                }
                else
                {
                    foreach (Collider2D coll in work_squares)
                    {
                        if (coll.bounds.Contains(transform.position))
                        {
                           if(transporting)
                            {
                                transporting = false;
                                transform.parent = null;
                                working = true;
                            }else
                            {
                                blue_comp -= food_process_rate;
                                green_comp += food_process_rate;

                                if(blue_comp > 0)
                                {

                                    m_SpriteRenderer.color = new Color(1, green_comp, blue_comp);
                                }else
                                {
                                    Debug.Log(" WORKED");
                                    working = false;
                                }
                            }
                            
                        }

                    }
                }

                m_SpriteRenderer.color = new Color(1, green_comp, blue_comp);
              //  spawnPrefabAt(pointer.Position);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            penetratio = true;

            transporter = col.gameObject.transform;
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            penetratio = false;
        }
    }
}