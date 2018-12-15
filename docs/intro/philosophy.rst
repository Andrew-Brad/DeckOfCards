Design Philosphy
================

Before digging into the code, it's recommended to better understand the philosophy behind engineering.

Everything we do should be in the name of software quality.

**Everything.**

Lean Principles
^^^^^^^^^^^^^^^

`Lean principles <https://www.lean.org/WhatsLean/Principles.cfm>`_ guide everything.  The tricky part is the fact that quality means different things to different people.

* **External Customers - End Users:** Your end users are the reason we all get paid.  Listen to them.

* **Internal Customers - Ops:** Nowadays with Devops, some lines are blurred here, but when it comes to production outages, it's rare a developer is called in to mess around with infrastructure.  Partner with your infrastructure team and ensure the software behaves like they would expect it to without any 'tribal knowledge' or hacks.

* **Internal Customers - Engineers:** Your codebase should get you excited about writing code.  If it doesn't, have a retrospective with teammates and get on track.  It should *not* be difficult to change (fragile), or prone to failure.  What was your first day like as a new developer on a codebase?  Was the product well documented, and the standards and expectations made clear?  Technical debt is something that needs to be measured, managed, or at least observed and refactored continually.

If you'd like to read about the history and interactions between Lean Manufacturing and Software Design, I'd highly recommend `Implementing Lean Software Development <https://www.amazon.com/Implementing-Lean-Software-Development-Concept/dp/0321437381/ref=pd_bxgy_14_img_2?_encoding=UTF8&pd_rd_i=0321437381&pd_rd_r=385946bc-f59f-11e8-b1af-c353f8636503&pd_rd_w=LKZJC&pd_rd_wg=ToauZ&pf_rd_i=desktop-dp-sims&pf_rd_m=ATVPDKIKX0DER&pf_rd_p=6725dbd6-9917-451d-beba-16af7874e407&pf_rd_r=2GT1SSH33DR82EMZ9X4B&pf_rd_s=desktop-dp-sims&pf_rd_t=40701&psc=1&refRID=2GT1SSH33DR82EMZ9X4B>`_.
Next, let's cover some principles already established in the industry.

S.O.L.I.D. Principles
^^^^^^^^^^^^^^^^^^^^^

WIP

Principles of Microservices
^^^^^^^^^^^^^^^^^^^^^^^^^^^
WIP

.. toctree::
   :maxdepth: 3
   :hidden:
   :caption: Design Philosophy

   philosophy/solid_principles
   philosophy/principles_of_microservices
   philosophy/software_quality_principles
   
   