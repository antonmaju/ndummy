Dummy
=====

Dummy is a .NET portable class library for generating object by its spec.

##Usage ##

<ul>
  <li>Create spec for classes you want to generate


    public class WarriorSpec : FactorySpec<Warrior>
    {
        public WarriorSpec()
        {
            ConfigureProperties(
               For(n =>n.Name).GenerateWith(ctx =>"Name-"+ctx.Index),
			   For(n =>n.Hp).GenerateWith(100) 	
              );
        }
    }


</li> 
<li>Register specs</li>	
	
    var config = new DummyConfig()
		.Register<Warrior, WarriorSpec>()
		.Register<Weapon, WeaponSpec>();

	var dummy = new Dummy(config);
    
<li>Get factory<li>
    
	var factory = dummy.GetFactory<Warrior>();
	newInstance = factory.Create(); //create new Warrior instance 

For detailed documentation please visit Dummy project pages.
