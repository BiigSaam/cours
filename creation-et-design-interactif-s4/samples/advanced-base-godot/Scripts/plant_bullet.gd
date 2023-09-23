extends RigidBody2D

@export var speed = 200;
@export var damage:int = 1

var direction: Vector2 = Vector2.LEFT
var has_collided = false
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")
var fra

func _ready():
	$Fragment1.visible = false
	$Fragment2.visible = false

func _on_visible_on_screen_notifier_2d_screen_exited():
	await get_tree().create_timer(1.2).timeout
	_on_auto_destroy_timeout()

func _on_body_entered(body):
	$AutoDestroy.start()
	linear_velocity = Vector2(0, gravity * 0.15)
	$Core.visible = false
	$Fragment1.visible = true
	$Fragment2.visible = true
	$CollisionShape2D.set_deferred("disabled", true)

	if "hit" in body:
		body.hit(damage)

func _on_auto_destroy_timeout():
	queue_free()
