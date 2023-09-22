extends RigidBody2D

@export var speed = 200;

var direction: Vector2 = Vector2.LEFT
var has_collided = false
@export var damage:int = 1

var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

func _ready():
	$Fragment1.visible = false
	$Fragment2.visible = false

#func _process(delta):
#	if !has_collided:
#		position += direction * speed * delta

func _on_visible_on_screen_notifier_2d_screen_exited():
	pass
#	await get_tree().create_timer(1.2).timeout
#	queue_free()


func _on_body_entered(body):
	$AutoDestroy.start()
	linear_velocity = Vector2(0, gravity * 0.15)
	$Core.visible = false
	$Fragment1.visible = true
	$Fragment2.visible = true
	if "hit" in body:
		body.hit(damage)

func _on_auto_destroy_timeout():
	queue_free()
